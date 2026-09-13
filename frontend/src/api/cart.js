import { apiRequest } from "./client";

export function getCart() {
  return apiRequest("/Cart");
}

export function addCartItem(payload) {
  return apiRequest("/Cart/items", {
    method: "POST",
    body: JSON.stringify(payload)
  });
}

export function setCartItemQuantity(bookId, payload) {
  return apiRequest(`/Cart/items/${bookId}`, {
    method: "PUT",
    body: JSON.stringify(payload)
  });
}

export function removeCartItem(bookId) {
  return apiRequest(`/Cart/items/${bookId}`, {
    method: "DELETE"
  });
}

export function clearCart() {
  return apiRequest("/Cart", {
    method: "DELETE"
  });
}
