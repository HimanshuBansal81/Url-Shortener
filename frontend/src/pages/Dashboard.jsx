import { useEffect, useState } from "react";
import api from "../services/api";

export default function Dashboard() {
  const [loading, setLoading] = useState(true);
  const [error, setError] = useState("");
  const [items, setItems] = useState([]);
  const [page, setPage] = useState(1);
  const [pageSize] = useState(10);
  const [totalCount, setTotalCount] = useState(0);
  const [deleteLoadingId, setDeleteLoadingId] = useState(null);
  const [analyticsLoadingId, setAnalyticsLoadingId] = useState(null);
  const [analytics, setAnalytics] = useState(null);

  const fetchUrls = async (showLoading = true) => {
    if (showLoading) {
      setLoading(true);
    }
    setError("");

    try {
      const response = await api.get(`/url/dashboard?page=${page}&pageSize=${pageSize}`);
      const data = response.data.data;
      setItems(data.items);
      setTotalCount(data.totalCount);
    } catch (apiError) {
      setError(
        apiError.response?.data?.message ||
          apiError.response?.data?.error ||
          "Unable to load dashboard."
      );
    } finally {
      if (showLoading) {
        setLoading(false);
      }
    }
  };

  useEffect(() => {
    fetchUrls();
  }, [page, pageSize]);

  useEffect(() => {
    const handleFocus = () => {
      fetchUrls(false);
    };

    window.addEventListener("focus", handleFocus);
    return () => window.removeEventListener("focus", handleFocus);
  }, [page, pageSize]);

  const totalPages = Math.max(1, Math.ceil(totalCount / pageSize));

  const handleDelete = async (id) => {
    const confirmed = window.confirm("Are you sure you want to delete this URL?");
    if (!confirmed) {
      return;
    }

    setDeleteLoadingId(id);
    setError("");

    try {
      await api.delete(`/url/${id}`);
      setItems((current) => current.filter((item) => item.id !== id));
      setTotalCount((current) => Math.max(0, current - 1));
    } catch (apiError) {
      setError(
        apiError.response?.data?.message ||
          apiError.response?.data?.error ||
          "Unable to delete URL."
      );
    } finally {
      setDeleteLoadingId(null);
    }
  };

  const handleViewAnalytics = async (id) => {
    setAnalyticsLoadingId(id);
    setError("");

    try {
      const response = await api.get(`/url/${id}/analytics`);
      setAnalytics(response.data.data);
    } catch (apiError) {
      setError(
        apiError.response?.data?.message ||
          apiError.response?.data?.error ||
          "Unable to load analytics."
      );
    } finally {
      setAnalyticsLoadingId(null);
    }
  };

  const closeAnalytics = async () => {
    setAnalytics(null);
    await fetchUrls(false);
  };

  const formatExpiry = (expiresAt) => {
    if (!expiresAt) {
      return "No expiry";
    }

    return new Date(expiresAt).toLocaleString();
  };

  const simplifyUserAgent = (userAgent) => {
    if (!userAgent) {
      return "-";
    }

    const value = userAgent.toLowerCase();

    let browser = "Unknown Browser";
    if (value.includes("edg/")) browser = "Edge";
    else if (value.includes("chrome") && !value.includes("edg/")) browser = "Chrome";
    else if (value.includes("safari") && !value.includes("chrome")) browser = "Safari";
    else if (value.includes("firefox")) browser = "Firefox";
    else if (value.includes("msie") || value.includes("trident")) browser = "Internet Explorer";

    let os = "Unknown OS";
    if (value.includes("iphone")) os = "iPhone";
    else if (value.includes("ipad")) os = "iPad";
    else if (value.includes("android")) os = "Android";
    else if (value.includes("windows")) os = "Windows";
    else if (value.includes("mac os") || value.includes("macintosh")) os = "macOS";
    else if (value.includes("linux")) os = "Linux";

    return `${browser} on ${os}`;
  };

  const getStatus = (expiresAt) => {
    if (!expiresAt) {
      return { label: "Active", className: "bg-emerald-100 text-emerald-700" };
    }

    return new Date(expiresAt) < new Date()
      ? { label: "Expired", className: "bg-rose-100 text-rose-700" }
      : { label: "Active", className: "bg-emerald-100 text-emerald-700" };
  };

  return (
    <div className="container mx-auto max-w-3xl px-4 py-10">
      <div className="space-y-4 rounded-xl bg-white p-4 shadow-md">
        <div className="flex items-center justify-between gap-4">
          <div>
            <h1 className="text-2xl font-semibold text-slate-900">Dashboard</h1>
            <p className="mt-2 text-sm text-slate-500">View the URLs you have created.</p>
          </div>
          <div className="text-sm text-slate-500">Total URLs: {totalCount}</div>
        </div>

        {loading ? <p className="mt-6 text-sm text-slate-500">Loading...</p> : null}
        {error ? <p className="mt-6 text-sm text-rose-600">{error}</p> : null}

        {!loading && !error && items.length === 0 ? (
          <div className="mt-6 rounded-xl border border-dashed border-slate-300 bg-slate-50 p-6 text-sm text-slate-500">
            No URLs yet. Create your first one 🚀
          </div>
        ) : null}

        {!loading && !error && items.length > 0 ? (
          <div className="mt-6 space-y-4">
            {items.map((item) => {
              const status = getStatus(item.expiresAt);

              return (
              <div key={item.id} className="space-y-4 rounded-xl border border-slate-200 p-4">
                <p className="text-xs font-semibold uppercase tracking-wide text-slate-500">Original URL</p>
                <p className="mt-1 break-all text-sm text-slate-700">{item.originalUrl}</p>

                <div className="mt-4 grid gap-3 sm:grid-cols-4">
                  <div>
                    <p className="text-xs font-semibold uppercase tracking-wide text-slate-500">Short URL</p>
                    <a
                      href={item.shortUrl}
                      target="_blank"
                      rel="noreferrer"
                      className="mt-1 block break-all text-sm font-medium text-blue-700 underline hover:text-blue-800"
                    >
                      {item.shortUrl}
                    </a>
                  </div>

                  <div>
                    <p className="text-xs font-semibold uppercase tracking-wide text-slate-500">Clicks</p>
                    <p className="mt-1 text-sm text-slate-700">{item.clickCount}</p>
                  </div>

                  <div>
                    <p className="text-xs font-semibold uppercase tracking-wide text-slate-500">Created</p>
                    <p className="mt-1 text-sm text-slate-700">
                      {new Date(item.createdAt).toLocaleString()}
                    </p>
                  </div>

                  <div>
                    <p className="text-xs font-semibold uppercase tracking-wide text-slate-500">Status</p>
                    <span className={`mt-1 inline-flex rounded-full px-2 py-1 text-xs font-medium ${status.className}`}>
                      {status.label}
                    </span>
                    <p className="mt-2 text-xs text-slate-500">Expires at: {formatExpiry(item.expiresAt)}</p>
                  </div>
                </div>

                <div className="mt-4 flex flex-wrap gap-3">
                  <button
                    type="button"
                    onClick={() => handleViewAnalytics(item.id)}
                    disabled={analyticsLoadingId === item.id}
                    className="rounded-lg bg-blue-600 px-4 py-2 text-sm font-medium text-white hover:bg-blue-700 disabled:cursor-not-allowed disabled:opacity-60"
                  >
                    {analyticsLoadingId === item.id ? "Loading..." : "View Analytics"}
                  </button>
                  <button
                    type="button"
                    onClick={() => handleDelete(item.id)}
                    disabled={deleteLoadingId === item.id}
                    className="rounded-lg bg-rose-600 px-4 py-2 text-sm font-medium text-white hover:bg-rose-700 disabled:cursor-not-allowed disabled:opacity-60"
                  >
                    {deleteLoadingId === item.id ? "Deleting..." : "Delete"}
                  </button>
                </div>
              </div>
            )})}

            <div className="flex items-center justify-between border-t border-slate-200 pt-4">
              <button
                type="button"
                onClick={() => setPage((current) => Math.max(1, current - 1))}
                disabled={page === 1}
                className="rounded-lg border border-slate-300 px-4 py-2 text-sm font-medium text-slate-700 hover:bg-slate-50 disabled:cursor-not-allowed disabled:opacity-50"
              >
                Prev
              </button>

              <p className="text-sm text-slate-500">
                Page {page} of {totalPages}
              </p>

              <button
                type="button"
                onClick={() => setPage((current) => Math.min(totalPages, current + 1))}
                disabled={page >= totalPages}
                className="rounded-lg border border-slate-300 px-4 py-2 text-sm font-medium text-slate-700 hover:bg-slate-50 disabled:cursor-not-allowed disabled:opacity-50"
              >
                Next
              </button>
            </div>
          </div>
        ) : null}
      </div>

      {analytics ? (
        <div className="fixed inset-0 z-50 flex items-center justify-center bg-slate-900/40 px-4">
          <div className="w-full max-w-2xl rounded-xl bg-white p-5 shadow-md">
            <div className="flex items-start justify-between gap-4">
              <div>
                <h2 className="text-lg font-semibold text-slate-900">Analytics</h2>
                <p className="mt-1 text-sm text-slate-500 break-all">{analytics.shortUrl}</p>
              </div>
              <button
                type="button"
                onClick={closeAnalytics}
                className="rounded-lg border border-slate-300 px-3 py-1 text-sm text-slate-700 hover:bg-slate-50"
              >
                Close
              </button>
            </div>

            <div className="mt-4 grid gap-3 sm:grid-cols-2">
              <div className="rounded-lg bg-slate-50 p-3">
                <p className="text-xs font-semibold uppercase tracking-wide text-slate-500">Total Clicks</p>
                <p className="mt-1 text-sm text-slate-800">{analytics.clickCount}</p>
              </div>
              <div className="rounded-lg bg-slate-50 p-3">
                <p className="text-xs font-semibold uppercase tracking-wide text-slate-500">Created</p>
                <p className="mt-1 text-sm text-slate-800">{new Date(analytics.createdAt).toLocaleString()}</p>
              </div>
            </div>

            <div className="mt-5">
              <h3 className="text-sm font-semibold text-slate-900">Recent Clicks</h3>

              {analytics.recentClicks.length === 0 ? (
                <p className="mt-3 text-sm text-slate-500">No clicks yet.</p>
              ) : (
                <div className="mt-3 max-h-80 overflow-auto rounded-lg border border-slate-200">
                  <table className="min-w-full divide-y divide-slate-200 text-sm">
                    <thead className="bg-slate-50">
                      <tr>
                        <th className="px-3 py-2 text-left font-medium text-slate-600">Timestamp</th>
                        <th className="px-3 py-2 text-left font-medium text-slate-600">IP Address</th>
                        <th className="px-3 py-2 text-left font-medium text-slate-600">User Agent</th>
                      </tr>
                    </thead>
                    <tbody className="divide-y divide-slate-200">
                      {analytics.recentClicks.map((click, index) => (
                        <tr key={`${click.clickedAt}-${index}`}>
                          <td className="px-3 py-2 text-slate-700">{new Date(click.clickedAt).toLocaleString()}</td>
                          <td className="px-3 py-2 text-slate-700">{click.ipAddress || "-"}</td>
                          <td className="px-3 py-2 text-slate-700 break-all">{simplifyUserAgent(click.userAgent)}</td>
                        </tr>
                      ))}
                    </tbody>
                  </table>
                </div>
              )}
            </div>
          </div>
        </div>
      ) : null}
    </div>
  );
}
