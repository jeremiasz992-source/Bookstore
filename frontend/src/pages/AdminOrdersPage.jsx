import { useEffect, useState } from "react";
import { getAllOrders, updateOrderStatus } from "../api/orders";
import { Message } from "../components/Message.jsx";
import { describeOrderStatus, describePaymentMethod, formatPrice } from "../labels.js";

function orderTotal(order) {
  return order.items.reduce((sum, item) => sum + item.price * item.quantity, 0);
}

export function AdminOrdersPage() {
  const [orders, setOrders] = useState([]);
  const [message, setMessage] = useState({ error: "", success: "" });

  async function loadOrders() {
    const data = await getAllOrders();
    setOrders(data);
  }

  useEffect(() => {
    loadOrders().catch((err) => setMessage({ error: err.message, success: "" }));
  }, []);

  async function handleComplete(orderId) {
    setMessage({ error: "", success: "" });
    try {
      await updateOrderStatus(orderId, "Completed");
      await loadOrders();
      setMessage({ error: "", success: `Zamówienie #${orderId} oznaczono jako zrealizowane.` });
    } catch (err) {
      setMessage({ error: err.message, success: "" });
    }
  }

  return (
    <section className="panel">
      <h1>Admin: zamówienia</h1>
      <Message error={message.error} success={message.success} />
      {!orders.length && <p>Brak zamówień.</p>}
      <table className="admin-table">
        <thead>
          <tr>
            <th>#</th>
            <th>Klient</th>
            <th>Data</th>
            <th>Status</th>
            <th>Forma płatności</th>
            <th>Pozycje</th>
            <th>Suma</th>
            <th>Akcje</th>
          </tr>
        </thead>
        <tbody>
          {orders.map((order) => (
            <tr key={order.orderId}>
              <td>{order.orderId}</td>
              <td>{order.userEmail || "-"}</td>
              <td>{new Date(order.createdAt).toLocaleString()}</td>
              <td>{describeOrderStatus(order.status)}</td>
              <td>{order.payment ? describePaymentMethod(order.payment.method) : "-"}</td>
              <td>
                {order.items.map((item) => (
                  <div key={`${order.orderId}-${item.bookId}`}>{item.title} x {item.quantity}</div>
                ))}
              </td>
              <td>{formatPrice(orderTotal(order))} PLN</td>
              <td>
                {order.status === "Paid" ? (
                  <button type="button" onClick={() => handleComplete(order.orderId)}>
                    Oznacz jako zrealizowane
                  </button>
                ) : (
                  "-"
                )}
              </td>
            </tr>
          ))}
        </tbody>
      </table>
    </section>
  );
}
