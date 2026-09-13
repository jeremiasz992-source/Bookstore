import { apiRequest } from "./client";

export function getBooks({ page = 1, pageSize = 10, categoryId = "", sort = "", search = "" } = {}) {
  const params = new URLSearchParams({ page, pageSize });
  if (categoryId) params.set("categoryId", categoryId);
  if (sort) params.set("sort", sort);
  if (search) params.set("search", search);
  return apiRequest(`/Books?${params.toString()}`);
}

export function createBook(payload) {
  return apiRequest("/Books", {
    method: "POST",
    body: JSON.stringify(payload)
  });
}

export function updateBook(id, payload) {
  return apiRequest(`/Books/${id}`, {
    method: "PUT",
    body: JSON.stringify(payload)
  });
}

export function deleteBook(id) {
  return apiRequest(`/Books/${id}`, {
    method: "DELETE"
  });
}
