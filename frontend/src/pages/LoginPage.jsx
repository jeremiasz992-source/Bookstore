import { useState } from "react";
import { useNavigate } from "react-router-dom";
import { Message } from "../components/Message.jsx";
import { useAuth } from "../context/AuthContext.jsx";

export function LoginPage() {
  const { signIn } = useAuth();
  const navigate = useNavigate();
  const [form, setForm] = useState({ email: "", password: "" });

  // Komunikat zapisany po wygaśnięciu sesji.
  const [error, setError] = useState(() => {
    try {
      const powod = sessionStorage.getItem("bookstore_wylogowano");
      if (powod) {
        sessionStorage.removeItem("bookstore_wylogowano");
        return powod;
      }
    } catch {
      //Ignorowanie błędu dostępu do sessionStorage.
    }
    return "";
  });

  async function handleSubmit(event) {
    event.preventDefault();
    setError("");

    if (!form.email.trim() || !form.password) {
      setError("Podaj adres e-mail oraz hasło.");
      return;
    }

    try {
      await signIn(form);
      navigate("/books");
    } catch (err) {
      setError(err.message);
    }
  }

  return (
    <section className="panel narrow">
      <h1>Logowanie</h1>
      <form onSubmit={handleSubmit} className="form" noValidate>
        <label>E-mail
          <input value={form.email} onChange={(e) => setForm({ ...form, email: e.target.value })} />
        </label>
        <label>Hasło
          <input type="password" value={form.password} onChange={(e) => setForm({ ...form, password: e.target.value })} />
        </label>
        <button type="submit">Zaloguj</button>
      </form>
      <Message error={error} />
    </section>
  );
}
