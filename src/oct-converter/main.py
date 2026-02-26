"""
OCT-Converter microservice.

Accepts native ophthalmic image files (Heidelberg .e2e, Topcon .fds/.fda,
Optovue .img, Bioptigen .img, Cirrus .img) and converts them to DICOM
using the open-source oct-converter library.

POST /convert
  Form fields:
    file   – the native image file
    format – file extension hint (e2e | fds | fda | img | sdb | oct)
  Returns the converted DICOM file as application/dicom.

GET /health
  Returns {"status": "ok"}.
"""

import io
import tempfile
import os
import logging
from pathlib import Path

from fastapi import FastAPI, File, Form, HTTPException, UploadFile
from fastapi.responses import Response, JSONResponse

app = FastAPI(title="OCT-Converter Service", version="1.0.0")
logger = logging.getLogger("oct-converter")
logging.basicConfig(level=logging.INFO)


@app.get("/health")
def health() -> dict:
    return {"status": "ok"}


@app.post("/convert")
async def convert(
    file: UploadFile = File(...),
    format: str = Form(...),
) -> Response:
    """Convert a proprietary ophthalmic image file to DICOM."""
    fmt = format.lower().lstrip(".")

    supported_formats = {"e2e", "fds", "fda", "img", "sdb", "oct"}
    if fmt not in supported_formats:
        raise HTTPException(
            status_code=422,
            detail=f"Unsupported format '{fmt}'. Supported: {sorted(supported_formats)}",
        )

    file_bytes = await file.read()
    if not file_bytes:
        raise HTTPException(status_code=400, detail="Uploaded file is empty.")

    with tempfile.TemporaryDirectory() as tmpdir:
        input_path = Path(tmpdir) / f"input.{fmt}"
        output_path = Path(tmpdir) / "output.dcm"

        input_path.write_bytes(file_bytes)

        try:
            dicom_bytes = _convert_to_dicom(input_path, output_path, fmt)
        except Exception as exc:
            logger.error("Conversion failed for format %s: %s", fmt, exc)
            raise HTTPException(
                status_code=500,
                detail=f"Conversion failed: {exc}",
            )

    return Response(
        content=dicom_bytes,
        media_type="application/dicom",
        headers={"Content-Disposition": "attachment; filename=converted.dcm"},
    )


def _convert_to_dicom(input_path: Path, output_path: Path, fmt: str) -> bytes:
    """Run the appropriate oct-converter reader and write DICOM output."""
    from oct_converter.readers import (
        E2E,
        FDA,
        FDS,
        IMG,
        OCT,
        SDB,
    )

    reader_map = {
        "e2e": E2E,
        "fds": FDS,
        "fda": FDA,
        "img": IMG,
        "sdb": SDB,
        "oct": OCT,
    }

    reader_cls = reader_map[fmt]
    reader = reader_cls(str(input_path))

    # oct_converter readers return a list of volume/fundus objects
    volumes = reader.read_oct_volume()
    if not volumes:
        raise ValueError("No OCT volumes found in file.")

    volume = volumes[0] if isinstance(volumes, list) else volumes

    # Write as DICOM
    volume.save(str(output_path))

    return output_path.read_bytes()
