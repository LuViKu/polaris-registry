"use client";

import { useState } from "react";
import { useForm } from "react-hook-form";
import { zodResolver } from "@hookform/resolvers/zod";
import { z } from "zod";
import Link from "next/link";

const schema = z.object({
  patientId: z
    .string()
    .uuid("Must be a valid patient UUID")
    .min(1, "Patient ID is required"),
  visitDate: z
    .string()
    .min(1, "Visit date is required")
    .refine(
      (d) => new Date(d) <= new Date(),
      "Visit date cannot be in the future",
    ),
  bestCorrectedVisualAcuity: z.string().max(50).optional().or(z.literal("")),
  intraocularPressure: z.string().max(50).optional().or(z.literal("")),
  clinicalNotes: z.string().optional().or(z.literal("")),
});

type FormValues = z.infer<typeof schema>;

export default function NewClinicalVisitPage() {
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

  async function onSubmit(values: FormValues) {
    setSubmitting(true);
    setApiError(null);
    setSuccess(null);

    try {
      const apiUrl =
        process.env.NEXT_PUBLIC_API_URL ?? "http://localhost:5000";
      const res = await fetch(`${apiUrl}/api/clinicalvisits`, {
        method: "POST",
        headers: { "Content-Type": "application/json" },
        body: JSON.stringify({
          patientId: values.patientId,
          visitDate: values.visitDate,
          bestCorrectedVisualAcuity: values.bestCorrectedVisualAcuity || null,
          intraocularPressure: values.intraocularPressure || null,
          clinicalNotes: values.clinicalNotes || null,
        }),
      });

      if (!res.ok) {
        const body = await res.json().catch(() => ({ error: res.statusText }));
        throw new Error(body?.error ?? `Request failed (${res.status})`);
      }

      const data = await res.json();
      setSuccess(
        `Clinical visit recorded successfully. Visit ID: ${data.id}. You can now upload imaging studies for this visit.`,
      );
      reset();
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
          / New Clinical Visit
        </nav>

        <h1 className="text-2xl font-bold text-gray-900">
          Record Clinical Visit
        </h1>
        <p className="mt-1 text-sm text-gray-600">
          Enter structured ophthalmic measurements collected during the patient
          visit.
        </p>

        {success && (
          <div className="mt-4 rounded-md bg-green-50 p-4 text-sm text-green-800 ring-1 ring-green-200">
            {success}{" "}
            <Link
              href="/upload"
              className="font-medium underline hover:text-green-900"
            >
              Upload imaging study →
            </Link>
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
          {/* Patient ID */}
          <div>
            <label
              htmlFor="patientId"
              className="block text-sm font-medium text-gray-700"
            >
              Patient ID <span className="text-red-500">*</span>
            </label>
            <input
              id="patientId"
              type="text"
              placeholder="e.g. 3fa85f64-5717-4562-b3fc-2c963f66afa6"
              className="mt-1 block w-full rounded-md border border-gray-300 px-3 py-2 text-sm shadow-sm focus:border-indigo-500 focus:outline-none focus:ring-1 focus:ring-indigo-500"
              {...register("patientId")}
            />
            {errors.patientId && (
              <p className="mt-1 text-xs text-red-600">
                {errors.patientId.message}
              </p>
            )}
          </div>

          {/* Visit Date */}
          <div>
            <label
              htmlFor="visitDate"
              className="block text-sm font-medium text-gray-700"
            >
              Visit Date <span className="text-red-500">*</span>
            </label>
            <input
              id="visitDate"
              type="date"
              className="mt-1 block w-full rounded-md border border-gray-300 px-3 py-2 text-sm shadow-sm focus:border-indigo-500 focus:outline-none focus:ring-1 focus:ring-indigo-500"
              {...register("visitDate")}
            />
            {errors.visitDate && (
              <p className="mt-1 text-xs text-red-600">
                {errors.visitDate.message}
              </p>
            )}
          </div>

          {/* Clinical Measurements */}
          <fieldset className="rounded-md border border-gray-200 p-4">
            <legend className="px-1 text-sm font-medium text-gray-700">
              Ophthalmic Measurements
            </legend>

            <div className="mt-3 grid gap-4 sm:grid-cols-2">
              {/* BCVA */}
              <div>
                <label
                  htmlFor="bcva"
                  className="block text-sm font-medium text-gray-700"
                >
                  Best Corrected Visual Acuity (BCVA)
                </label>
                <input
                  id="bcva"
                  type="text"
                  placeholder="e.g. 6/6 or 20/20"
                  className="mt-1 block w-full rounded-md border border-gray-300 px-3 py-2 text-sm shadow-sm focus:border-indigo-500 focus:outline-none focus:ring-1 focus:ring-indigo-500"
                  {...register("bestCorrectedVisualAcuity")}
                />
                {errors.bestCorrectedVisualAcuity && (
                  <p className="mt-1 text-xs text-red-600">
                    {errors.bestCorrectedVisualAcuity.message}
                  </p>
                )}
              </div>

              {/* IOP */}
              <div>
                <label
                  htmlFor="iop"
                  className="block text-sm font-medium text-gray-700"
                >
                  Intraocular Pressure (IOP)
                </label>
                <input
                  id="iop"
                  type="text"
                  placeholder="e.g. 14 mmHg"
                  className="mt-1 block w-full rounded-md border border-gray-300 px-3 py-2 text-sm shadow-sm focus:border-indigo-500 focus:outline-none focus:ring-1 focus:ring-indigo-500"
                  {...register("intraocularPressure")}
                />
                {errors.intraocularPressure && (
                  <p className="mt-1 text-xs text-red-600">
                    {errors.intraocularPressure.message}
                  </p>
                )}
              </div>
            </div>
          </fieldset>

          {/* Clinical Notes */}
          <div>
            <label
              htmlFor="clinicalNotes"
              className="block text-sm font-medium text-gray-700"
            >
              Clinical Notes
            </label>
            <textarea
              id="clinicalNotes"
              rows={4}
              placeholder="Free-text clinical observations…"
              className="mt-1 block w-full rounded-md border border-gray-300 px-3 py-2 text-sm shadow-sm focus:border-indigo-500 focus:outline-none focus:ring-1 focus:ring-indigo-500"
              {...register("clinicalNotes")}
            />
          </div>

          <button
            type="submit"
            disabled={submitting}
            className="w-full rounded-md bg-indigo-600 px-4 py-2 text-sm font-semibold text-white shadow-sm hover:bg-indigo-500 focus:outline-none focus:ring-2 focus:ring-indigo-600 disabled:opacity-50"
          >
            {submitting ? "Saving…" : "Save Clinical Visit"}
          </button>
        </form>
      </div>
    </main>
  );
}
