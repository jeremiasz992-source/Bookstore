const API_BASE_URL = import.meta.env.VITE_API_BASE_URL || "http://localhost:5172/api";

function getToken() {
  return localStorage.getItem("bookstore_token");
}

export async function apiRequest(path, options = {}) {
  const headers = {
    "Content-Type": "application/json",
    ...(options.headers || {})
  };

  const token = getToken();
  if (token) {
    headers.Authorization = `Bearer ${token}`;
  }

  const response = await fetch(`${API_BASE_URL}${path}`, {
    ...options,
    headers
  });

  const text = await response.text();
  let data = null;
  try {
    data = text ? JSON.parse(text) : null;
  } catch {
    data = text;
  }

  // Token wygasa po czasie określonym w konfiguracji. Odpowiedź 401 przy wysłanym
  // tokenie oznacza, że sesja straciła ważność - dane logowania są wtedy usuwane,
  // a użytkownik kierowany do formularza logowania.
  if (response.status === 401 && token) {
    localStorage.removeItem("bookstore_token");
    localStorage.removeItem("bookstore_auth");
    try {
      sessionStorage.setItem("bookstore_wylogowano", "Sesja wygasła. Zaloguj się ponownie.");
    } catch {
      // Brak dostępu do pamięci przeglądarki oznacza tylko brak komunikatu.
    }
    if (!window.location.pathname.startsWith("/login")) {
      window.location.assign("/login");
    }
    throw new Error("Sesja wygasła. Zaloguj się ponownie.");
  }

  if (!response.ok) {
    if (data?.errors && typeof data.errors === "object") {
      const messages = Object.values(data.errors)
        .flat()
        .filter(message => typeof message === "string" && message.trim());
      if (messages.length > 0) {
        throw new Error(messages.join(" "));
      }
    }

    const message = data?.message || data || `HTTP ${response.status}`;
    throw new Error(typeof message === "string" ? message : JSON.stringify(message));
  }

  return data;
}
