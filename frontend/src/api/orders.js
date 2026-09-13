import { apiRequest } from "./client";

export function getOrders() {
  return apiRequest("/Orders");
}

export function getAllOrders() {
  return apiRequest("/Orders/all");
}

export function checkout() {
  return apiRequest("/Orders/checkout", {
    method: "POST"
  });
}

export function updateOrderStatus(orderId, status) {
  return apiRequest(`/Orders/${orderId}/status`, {
    method: "PUT",
    body: JSON.stringify({ status })
  });
}

export function payOrder(orderId, method = "Card") {
  return apiRequest(`/Orders/${orderId}/pay`, {
    method: "POST",
    body: JSON.stringify({ method })
  });
}
