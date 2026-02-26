import Link from "next/link";

export default function DashboardPage() {
  return (
    <main className="min-h-screen bg-gray-50 p-8">
      <div className="mx-auto max-w-7xl">
        <h1 className="text-3xl font-bold text-gray-900">
          Ophthalmic Registry Dashboard
        </h1>
        <p className="mt-2 text-gray-600">
          Welcome to the Standardized Multimodal Ophthalmic Data Registry.
        </p>

        <div className="mt-8 grid grid-cols-1 gap-6 sm:grid-cols-2 lg:grid-cols-4">
          {[
            { label: "Total Patients", value: "—" },
            { label: "Clinical Visits", value: "—" },
            { label: "Imaging Studies", value: "—" },
            { label: "Active Sites", value: "—" },
          ].map((stat) => (
            <div
              key={stat.label}
              className="rounded-lg bg-white p-6 shadow-sm ring-1 ring-gray-200"
            >
              <dt className="text-sm font-medium text-gray-500">{stat.label}</dt>
              <dd className="mt-1 text-3xl font-semibold text-gray-900">
                {stat.value}
              </dd>
            </div>
          ))}
        </div>

        <div className="mt-10">
          <h2 className="text-lg font-semibold text-gray-900">Quick Actions</h2>
          <div className="mt-4 grid grid-cols-1 gap-4 sm:grid-cols-2 lg:grid-cols-3">
            <Link
              href="/visits/new"
              className="flex items-start gap-4 rounded-lg bg-white p-5 shadow-sm ring-1 ring-gray-200 hover:ring-indigo-400 transition-all"
            >
              <span className="flex h-10 w-10 shrink-0 items-center justify-center rounded-full bg-indigo-50 text-indigo-600">
                <svg xmlns="http://www.w3.org/2000/svg" className="h-5 w-5" viewBox="0 0 20 20" fill="currentColor">
                  <path fillRule="evenodd" d="M10 3a1 1 0 011 1v5h5a1 1 0 110 2h-5v5a1 1 0 11-2 0v-5H4a1 1 0 110-2h5V4a1 1 0 011-1z" clipRule="evenodd" />
                </svg>
              </span>
              <div>
                <p className="font-medium text-gray-900">Record Clinical Visit</p>
                <p className="mt-1 text-sm text-gray-500">
                  Add BCVA, IOP, and clinical notes for a patient visit.
                </p>
              </div>
            </Link>

            <Link
              href="/upload"
              className="flex items-start gap-4 rounded-lg bg-white p-5 shadow-sm ring-1 ring-gray-200 hover:ring-indigo-400 transition-all"
            >
              <span className="flex h-10 w-10 shrink-0 items-center justify-center rounded-full bg-indigo-50 text-indigo-600">
                <svg xmlns="http://www.w3.org/2000/svg" className="h-5 w-5" viewBox="0 0 20 20" fill="currentColor">
                  <path fillRule="evenodd" d="M3 17a1 1 0 011-1h12a1 1 0 110 2H4a1 1 0 01-1-1zM6.293 6.707a1 1 0 010-1.414l3-3a1 1 0 011.414 0l3 3a1 1 0 01-1.414 1.414L11 5.414V13a1 1 0 11-2 0V5.414L7.707 6.707a1 1 0 01-1.414 0z" clipRule="evenodd" />
                </svg>
              </span>
              <div>
                <p className="font-medium text-gray-900">Upload Imaging Study</p>
                <p className="mt-1 text-sm text-gray-500">
                  Upload DICOM or native ophthalmic files (OCT, fundus, FAF).
                </p>
              </div>
            </Link>
          </div>
        </div>
      </div>
    </main>
  );
}

