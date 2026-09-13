export const paymentMethods = [
  { value: "Card", label: "Karta płatnicza" },
  { value: "Blik", label: "BLIK" },
  { value: "Transfer", label: "Przelew bankowy" }
];

const orderStatusLabels = {
  Pending: "Nieopłacone",
  Paid: "Opłacone",
  Completed: "Zrealizowane"
};

export function describeOrderStatus(status) {
  return orderStatusLabels[status] || status;
}

export function describePaymentMethod(method) {
  return paymentMethods.find((item) => item.value === method)?.label || method;
}

export function formatPrice(value) {
  return Number(value).toLocaleString("pl-PL", {
    minimumFractionDigits: 2,
    maximumFractionDigits: 2
  });
}
