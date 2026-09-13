import { useState } from "react";
import { useNavigate } from "react-router-dom";
import { Message } from "../components/Message.jsx";
import { useAuth } from "../context/AuthContext.jsx";

const emailPattern = /^[^\s@]+@[^\s@]+\.[^\s@]+$/;

function validateForm({ email, password }) {
  if (!email.trim()) return "Podaj adres e-mail.";
  if (!emailPattern.test(email.trim())) return "Podaj poprawny adres e-mail, na przykład jan.kowalski@example.com.";
  if (!password) return "Podaj hasło.";
  if (password.length < 6) return "Hasło musi mieć co najmniej 6 znaków.";
  return "";
}

export function RegisterPage() {
  const { signUp } = useAuth();
  const navigate = useNavigate();
  const [form, setForm] = useState({ email: "", password: "" });
  const [error, setError] = useState("");

  async function handleSubmit(event) {
    event.preventDefault();
    setError("");

    const validationError = validateForm(form);
    if (validationError) {
      setError(validationError);
      return;
    }

    try {
      await signUp(form);
      navigate("/books");
    } catch (err) {
      setError(err.message);
    }
  }

  return (
    <section className="panel narrow">
      <h1>Rejestracja</h1>
      <form onSubmit={handleSubmit} className="form" noValidate>
        <label>E-mail
          <input value={form.email} onChange={(e) => setForm({ ...form, email: e.target.value })} />
        </label>
        <label>Hasło
          <input type="password" value={form.password} onChange={(e) => setForm({ ...form, password: e.target.value })} />
        </label>
        <button type="submit">Utwórz konto</button>
      </form>
      <Message error={error} />
    </section>
  );
}
