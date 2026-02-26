"use client";

import { useState } from "react";
import { useForm } from "react-hook-form";
import { zodResolver } from "@hookform/resolvers/zod";
import { z } from "zod";
import Link from "next/link";

const ACCEPTED_EXTENSIONS = [
  ".dcm",
  ".dicom",
  ".e2e",
  ".fds",
  ".fda",
  ".img",
  ".sdb",
  ".oct",
  ".png",
  ".jpg",
  ".jpeg",
  ".tiff",
];

const MODALITIES = ["OCT", "FP", "FAF", "ICG", "FFA", "OCTA", "Other"];

const schema = z.object({
  clinicalVisitId: z
    .string()
    .uuid("Must be a valid visit UUID")
    .min(1, "Clinical visit ID is required"),
  modality: z.string().min(1, "Modality is required"),
  acquiredAt: z.string().optional(),
});

type FormValues = z.infer<typeof schema>;

export default function UploadImagingPage() {
  const [file, setFile] = useState<File | null>(null);
  const [fileError, setFileError] = useState<string | null>(null);
  const [submitting, setSubmitting] = useState(false);
  const [success, setSuccess] = useState<string | null>(null);
  const [apiError, setApiError] = useState<string | null>(null);

  const {
    register,
    handleSubmit,
    reset,
    formState: { errors },
  } = useForm<FormValues>({
    resolver: zodResolver(schema),
  });

  function handleFileChange(e: React.ChangeEvent<HTMLInputElement>) {
    setFileError(null);
    const selected = e.target.files?.[0] ?? null;
    if (!selected) {
      setFile(null);
      return;
    }
    const ext = "." + selected.name.split(".").pop()?.toLowerCase();
    if (!ACCEPTED_EXTENSIONS.includes(ext)) {
      setFileError(
        `Unsupported file type "${ext}". Accepted: ${ACCEPTED_EXTENSIONS.join(", ")}`,
      );
      setFile(null);
      return;
    }
    setFile(selected);
  }

  async function onSubmit(values: FormValues) {
    if (!file) {
      setFileError("Please select an imaging file.");
      return;
    }
    setSubmitting(true);
    setApiError(null);
    setSuccess(null);

    try {
      const formData = new FormData();
      formData.append("ClinicalVisitId", values.clinicalVisitId);
      formData.append("Modality", values.modality);
      formData.append("File", file, file.name);
      if (values.acquiredAt) {
        formData.append("AcquiredAt", new Date(values.acquiredAt).toISOString());
      }

      const apiUrl =
        process.env.NEXT_PUBLIC_API_URL ?? "http://localhost:5000";
      const res = await fetch(`${apiUrl}/api/imaging/upload`, {
        method: "POST",
        body: formData,
      });

      if (!res.ok) {
        const body = await res.json().catch(() => ({ error: res.statusText }));
        throw new Error(body?.error ?? `Upload failed (${res.status})`);
      }

      const data = await res.json();
      setSuccess(`Study uploaded successfully. ID: ${data.id}`);
      reset();
      setFile(null);
    } catch (err: unknown) {
      setApiError(err instanceof Error ? err.message : "Unexpected error");
    } finally {
      setSubmitting(false);
    }
  }

  return (
    <main className="min-h-screen bg-gray-50 p-8">
      <div className="mx-auto max-w-2xl">
        <nav className="mb-6 text-sm text-gray-500">
          <Link href="/dashboard" className="hover:underline">
            Dashboard
          </Link>{" "}
          / Upload Imaging Study
        </nav>

        <h1 className="text-2xl font-bold text-gray-900">
          Upload Imaging Study
        </h1>
        <p className="mt-1 text-sm text-gray-600">
          Upload DICOM files or native ophthalmic formats (Heidelberg .e2e,
          Topcon .fds/.fda, Optovue .img, etc.). Native formats are
          automatically converted to DICOM where possible.
        </p>

        {success && (
          <div className="mt-4 rounded-md bg-green-50 p-4 text-sm text-green-800 ring-1 ring-green-200">
            {success}
          </div>
        )}
        {apiError && (
          <div className="mt-4 rounded-md bg-red-50 p-4 text-sm text-red-800 ring-1 ring-red-200">
            {apiError}
          </div>
        )}

        <form
          onSubmit={handleSubmit(onSubmit)}
          className="mt-6 space-y-6 rounded-lg bg-white p-6 shadow-sm ring-1 ring-gray-200"
          noValidate
        >
          {/* Clinical Visit ID */}
          <div>
            <label
              htmlFor="clinicalVisitId"
              className="block text-sm font-medium text-gray-700"
            >
              Clinical Visit ID <span className="text-red-500">*</span>
            </label>
            <input
              id="clinicalVisitId"
              type="text"
              placeholder="e.g. 3fa85f64-5717-4562-b3fc-2c963f66afa6"
              className="mt-1 block w-full rounded-md border border-gray-300 px-3 py-2 text-sm shadow-sm focus:border-indigo-500 focus:outline-none focus:ring-1 focus:ring-indigo-500"
              {...register("clinicalVisitId")}
            />
            {errors.clinicalVisitId && (
              <p className="mt-1 text-xs text-red-600">
                {errors.clinicalVisitId.message}
              </p>
            )}
          </div>

          {/* Modality */}
          <div>
            <label
              htmlFor="modality"
              className="block text-sm font-medium text-gray-700"
            >
              Imaging Modality <span className="text-red-500">*</span>
            </label>
            <select
              id="modality"
              className="mt-1 block w-full rounded-md border border-gray-300 bg-white px-3 py-2 text-sm shadow-sm focus:border-indigo-500 focus:outline-none focus:ring-1 focus:ring-indigo-500"
              {...register("modality")}
            >
              <option value="">Select modality…</option>
              {MODALITIES.map((m) => (
                <option key={m} value={m}>
                  {m}
                </option>
              ))}
            </select>
            {errors.modality && (
              <p className="mt-1 text-xs text-red-600">
                {errors.modality.message}
              </p>
            )}
          </div>

          {/* File Upload */}
          <div>
            <label
              htmlFor="file"
              className="block text-sm font-medium text-gray-700"
            >
              Imaging File <span className="text-red-500">*</span>
            </label>
            <div className="mt-1 flex justify-center rounded-md border-2 border-dashed border-gray-300 px-6 py-8 hover:border-indigo-400 transition-colors">
              <div className="text-center">
                <svg
                  className="mx-auto h-10 w-10 text-gray-400"
                  stroke="currentColor"
                  fill="none"
                  viewBox="0 0 48 48"
                  aria-hidden="true"
                >
                  <path
                    d="M28 8H12a4 4 0 00-4 4v20m32-12v8m0 0v8a4 4 0 01-4 4H12a4 4 0 01-4-4v-4m32-4l-3.172-3.172a4 4 0 00-5.656 0L28 28M8 32l9.172-9.172a4 4 0 015.656 0L28 28m0 0l4 4m4-24h8m-4-4v8m-12 4h.02"
                    strokeWidth={2}
                    strokeLinecap="round"
                    strokeLinejoin="round"
                  />
                </svg>
                <div className="mt-2 flex text-sm text-gray-600 justify-center">
                  <label
                    htmlFor="file"
                    className="cursor-pointer rounded-md font-medium text-indigo-600 hover:text-indigo-500"
                  >
                    Choose file
                    <input
                      id="file"
                      type="file"
                      className="sr-only"
                      accept={ACCEPTED_EXTENSIONS.join(",")}
                      onChange={handleFileChange}
                    />
                  </label>
                  <p className="pl-1">or drag and drop</p>
                </div>
                {file ? (
                  <p className="mt-2 text-sm font-medium text-indigo-700">
                    {file.name} ({(file.size / 1024 / 1024).toFixed(2)} MB)
                  </p>
                ) : (
                  <p className="mt-1 text-xs text-gray-500">
                    DICOM (.dcm), OCT native (.e2e, .fds, .fda, .img, .sdb,
                    .oct), or image (.png, .jpg, .tiff) up to 500 MB
                  </p>
                )}
              </div>
            </div>
            {fileError && (
              <p className="mt-1 text-xs text-red-600">{fileError}</p>
            )}
          </div>

          {/* Acquisition Date */}
          <div>
            <label
              htmlFor="acquiredAt"
              className="block text-sm font-medium text-gray-700"
            >
              Acquisition Date &amp; Time
            </label>
            <input
              id="acquiredAt"
              type="datetime-local"
              className="mt-1 block w-full rounded-md border border-gray-300 px-3 py-2 text-sm shadow-sm focus:border-indigo-500 focus:outline-none focus:ring-1 focus:ring-indigo-500"
              {...register("acquiredAt")}
            />
            <p className="mt-1 text-xs text-gray-500">
              Leave blank to use the current time.
            </p>
          </div>

          <button
            type="submit"
            disabled={submitting}
            className="w-full rounded-md bg-indigo-600 px-4 py-2 text-sm font-semibold text-white shadow-sm hover:bg-indigo-500 focus:outline-none focus:ring-2 focus:ring-indigo-600 disabled:opacity-50"
          >
            {submitting ? "Uploading…" : "Upload Study"}
          </button>
        </form>
      </div>
    </main>
  );
}
