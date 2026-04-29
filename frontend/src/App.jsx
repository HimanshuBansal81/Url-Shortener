import { Navigate, Route, Routes, useLocation } from "react-router-dom";
import Navbar from "./components/Navbar";
import Auth from "./pages/Auth";
import Shortener from "./pages/Shortener";
import Dashboard from "./pages/Dashboard";

function ProtectedRoute({ children }) {
  const token = localStorage.getItem("token");

  if (!token) {
    return <Navigate to="/auth" replace />;
  }

  return children;
}

export default function App() {
  const location = useLocation();
  const isLoggedIn = Boolean(localStorage.getItem("token"));
  const showNavbar = isLoggedIn && location.pathname !== "/auth";

  return (
    <div className="min-h-screen bg-slate-50">
      {showNavbar ? <Navbar /> : null}

      <Routes>
        <Route path="/" element={<Navigate to={isLoggedIn ? "/shorten" : "/auth"} replace />} />
        <Route path="/auth" element={<Auth />} />
        <Route
          path="/shorten"
          element={
            <ProtectedRoute>
              <Shortener />
            </ProtectedRoute>
          }
        />
        <Route
          path="/dashboard"
          element={
            <ProtectedRoute>
              <Dashboard />
            </ProtectedRoute>
          }
        />
        <Route path="*" element={<Navigate to={isLoggedIn ? "/shorten" : "/auth"} replace />} />
      </Routes>
    </div>
  );
}
