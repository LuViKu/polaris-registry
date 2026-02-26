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
      </div>
    </main>
  );
}
