import { useState } from "react";
import { QRCodeSVG } from "qrcode.react";
import api from "../services/api";

const initialResult = null;

export default function Shortener() {
  const [originalUrl, setOriginalUrl] = useState("");
  const [customAlias, setCustomAlias] = useState("");
  const [expiryOption, setExpiryOption] = useState("");
  const [result, setResult] = useState(initialResult);
  const [autoCopy, setAutoCopy] = useState(true);
  const [copied, setCopied] = useState(false);
  const [loading, setLoading] = useState(false);
  const [error, setError] = useState("");

  const handleShorten = async (event) => {
    event.preventDefault();
    setLoading(true);
    setError("");
    setCopied(false);

    try {
      const expiresAt =
        expiryOption === ""
          ? null
          : new Date(Date.now() + Number(expiryOption) * 24 * 60 * 60 * 1000).toISOString();

      const response = await api.post("/url", {
        originalUrl,
        customAlias: customAlias || null,
        expiresAt,
      });
      const data = response.data.data;

      setResult(data);
      setOriginalUrl("");
      setCustomAlias("");
      setExpiryOption("");

      if (autoCopy) {
        await navigator.clipboard.writeText(data.shortUrl);
        setCopied(true);
      }
    } catch (apiError) {
      if (apiError.response?.status === 409) {
        setError("This alias is already taken. Try another.");
        return;
      }

      setError(
        apiError.response?.data?.message ||
          apiError.response?.data?.error ||
          "Unable to shorten URL right now."
      );
    } finally {
      setLoading(false);
    }
  };

  const formatExpiry = (expiresAt) => {
    if (!expiresAt) {
      return "No expiry";
    }

    return new Date(expiresAt).toLocaleString();
  };

  const handleCopy = async () => {
    if (!result?.shortUrl) {
      return;
    }

    await navigator.clipboard.writeText(result.shortUrl);
    setCopied(true);
    window.setTimeout(() => setCopied(false), 2000);
  };

  return (
    <div className="container mx-auto max-w-3xl px-4 py-10">
      <div className="grid gap-6 lg:grid-cols-[1.3fr_0.7fr]">
        <section className="space-y-4 rounded-xl bg-white p-4 shadow-md">
          <h1 className="text-2xl font-semibold text-slate-900">Shorten a URL</h1>
          <p className="mt-2 text-sm text-slate-500">Paste a long link and generate a short URL instantly.</p>

          <form onSubmit={handleShorten} className="mt-6 space-y-4">
            <div>
              <label className="mb-1 block text-sm font-medium text-slate-700">Long URL</label>
              <input
                type="url"
                value={originalUrl}
                onChange={(event) => setOriginalUrl(event.target.value)}
                required
                placeholder="https://example.com/very/long/link"
                className="w-full rounded-lg border border-slate-300 px-4 py-3 outline-none focus:border-slate-900"
              />
            </div>

            <div>
              <label className="mb-1 block text-sm font-medium text-slate-700">Custom Alias (optional)</label>
              <input
                type="text"
                value={customAlias}
                onChange={(event) => setCustomAlias(event.target.value)}
                placeholder="my-custom-link"
                className="w-full rounded-lg border border-slate-300 px-4 py-3 outline-none focus:border-blue-600"
              />
            </div>

            <div>
              <label className="mb-1 block text-sm font-medium text-slate-700">Expiry (optional)</label>
              <select
                value={expiryOption}
                onChange={(event) => setExpiryOption(event.target.value)}
                className="w-full rounded-lg border border-slate-300 px-4 py-3 outline-none focus:border-blue-600"
              >
                <option value="">No expiry</option>
                <option value="1">1 day</option>
                <option value="7">7 days</option>
                <option value="30">30 days</option>
              </select>
            </div>

            <label className="flex items-center gap-2 text-sm text-slate-600">
              <input
                type="checkbox"
                checked={autoCopy}
                onChange={(event) => setAutoCopy(event.target.checked)}
                className="h-4 w-4 rounded border-slate-300"
              />
              Auto-copy after shorten
            </label>

            {error ? <p className="text-sm text-rose-600">{error}</p> : null}

            <button
              type="submit"
              disabled={loading}
              className="rounded-lg bg-blue-600 px-5 py-3 text-sm font-medium text-white hover:bg-blue-700 disabled:cursor-not-allowed disabled:opacity-60"
            >
              {loading ? "Shortening..." : "Shorten"}
            </button>
          </form>
        </section>

        <section className="space-y-4 rounded-xl bg-white p-4 shadow-md">
          <h2 className="text-lg font-semibold text-slate-900">Result</h2>

          {result ? (
            <div className="mt-5 space-y-4">
              <div className="rounded-xl border border-emerald-200 bg-emerald-50 p-4">
                <p className="text-xs font-semibold uppercase tracking-wide text-emerald-700">Short URL</p>
                <a
                  href={result.shortUrl}
                  target="_blank"
                  rel="noreferrer"
                  className="mt-2 block break-all text-lg font-semibold text-blue-700 underline"
                >
                  {result.shortUrl}
                </a>
                <p className="mt-2 text-sm text-slate-700">
                  Expires at: <span className="font-medium">{formatExpiry(result.expiresAt)}</span>
                </p>
                {copied ? <p className="mt-2 text-sm text-emerald-700">Copied!</p> : null}
              </div>

              <div className="flex flex-wrap gap-3">
                <button
                  type="button"
                  onClick={handleCopy}
                  className="rounded-lg bg-blue-600 px-4 py-2 text-sm font-medium text-white hover:bg-blue-700"
                >
                  Copy
                </button>
                <a
                  href={result.shortUrl}
                  target="_blank"
                  rel="noreferrer"
                  className="rounded-lg border border-slate-300 px-4 py-2 text-sm font-medium text-slate-700 hover:bg-slate-50"
                >
                  Open link
                </a>
              </div>

              <div className="flex justify-center rounded-xl border border-slate-200 bg-slate-50 p-4">
                <QRCodeSVG value={result.shortUrl} size={180} />
              </div>
            </div>
          ) : (
            <div className="mt-5 rounded-xl border border-dashed border-slate-300 bg-slate-50 p-6 text-sm text-slate-500">
              Your generated short URL and QR code will appear here.
            </div>
          )}
        </section>
      </div>
    </div>
  );
}
