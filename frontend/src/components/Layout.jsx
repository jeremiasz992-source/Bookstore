import { NavLink, Outlet } from "react-router-dom";
import { useAuth } from "../context/AuthContext.jsx";

export function Layout() {
  const { user, isAdmin, signOut } = useAuth();

  return (
    <div className="app-shell">
      <header className="topbar">
        <NavLink className="brand" to="/books">Bookstore</NavLink>
        <nav className="nav">
          <NavLink to="/books">Książki</NavLink>
          {user && <NavLink to="/cart">Koszyk</NavLink>}
          {user && <NavLink to="/orders">Zamówienia</NavLink>}
          {isAdmin && <NavLink to="/admin/books">Admin książki</NavLink>}
          {isAdmin && <NavLink to="/admin/categories">Admin kategorie</NavLink>}
          {isAdmin && <NavLink to="/admin/orders">Admin zamówienia</NavLink>}
        </nav>
        <div className="auth-nav">
          {user ? (
            <>
              <span>{user.email} ({user.role})</span>
              <button type="button" onClick={signOut}>Wyloguj</button>
            </>
          ) : (
            <>
              <NavLink to="/login">Logowanie</NavLink>
              <NavLink to="/register">Rejestracja</NavLink>
            </>
          )}
        </div>
      </header>
      <main className="page">
        <Outlet />
      </main>
      <footer className="footer">
        <div className="footer-inner">
          <span>&copy; {new Date().getFullYear()} Bookstore</span>
          <span className="footer-note">Internetowa księgarnia — praca inżynierska</span>
        </div>
      </footer>
    </div>
  );
}
