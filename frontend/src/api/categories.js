import { apiRequest } from "./client";

export function getCategories() {
  return apiRequest("/Categories");
}

export function createCategory(payload) {
  return apiRequest("/Categories", {
    method: "POST",
    body: JSON.stringify(payload)
  });
}

export function updateCategory(id, payload) {
  return apiRequest(`/Categories/${id}`, {
    method: "PUT",
    body: JSON.stringify(payload)
  });
}

export function deleteCategory(id) {
  return apiRequest(`/Categories/${id}`, {
    method: "DELETE"
  });
}
