import { apiRequest } from "./client";

export function login(payload) {
  return apiRequest("/Auth/login", {
    method: "POST",
    body: JSON.stringify(payload)
  });
}

export function register(payload) {
  return apiRequest("/Auth/register", {
    method: "POST",
    body: JSON.stringify(payload)
  });
}
