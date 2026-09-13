import { useEffect, useState } from "react";
import { getOrders, payOrder } from "../api/orders";
import { Message } from "../components/Message.jsx";
import { describeOrderStatus, describePaymentMethod, formatPrice, paymentMethods } from "../labels.js";

function paymentMessage(orderId, method) {
  return `Opłacono zamówienie #${orderId} — ${describePaymentMethod(method)}. Zamówienie zostanie przygotowane do wysyłki.`;
}

export function OrdersPage() {
  const [orders, setOrders] = useState([]);
  const [selectedMethods, setSelectedMethods] = useState({});
  const [message, setMessage] = useState({ error: "", success: "" });

  async function loadOrders() {
    const data = await getOrders();
    setOrders(data);
  }

  useEffect(() => {
    loadOrders().catch((err) => setMessage({ error: err.message, success: "" }));
  }, []);

  async function handlePay(orderId) {
    const method = selectedMethods[orderId] || "Card";
    setMessage({ error: "", success: "" });
    try {
      await payOrder(orderId, method);
      await loadOrders();
      setMessage({ error: "", success: paymentMessage(orderId, method) });
    } catch (err) {
      setMessage({ error: err.message, success: "" });
    }
  }

  return (
    <section className="panel">
      <h1>Zamówienia</h1>
      <Message error={message.error} success={message.success} />
      {!orders.length && <p>Nie masz jeszcze zamówień.</p>}
      <div className="list">
        {orders.map((order) => {
          const isSettled = order.payment?.status === "Paid";

          return (
            <article className="order" key={order.orderId}>
              <div className="row-between">
                <h2>#{order.orderId}</h2>
                <span>{describeOrderStatus(order.status)}</span>
              </div>
              {order.items.map((item) => (
                <p key={`${order.orderId}-${item.bookId}`}>{item.title} x {item.quantity} - {formatPrice(item.price * item.quantity)} PLN</p>
              ))}

              {isSettled ? (
                <p className="payment-info">
                  Opłacono: {describePaymentMethod(order.payment.method)}
                </p>
              ) : (
                <div className="payment-actions">
                  <label>
                    Forma płatności
                    <select
                      value={selectedMethods[order.orderId] || "Card"}
                      onChange={(e) => setSelectedMethods({ ...selectedMethods, [order.orderId]: e.target.value })}
                    >
                      {paymentMethods.map((method) => (
                        <option key={method.value} value={method.value}>{method.label}</option>
                      ))}
                    </select>
                  </label>
                  <button type="button" onClick={() => handlePay(order.orderId)}>
                    Zapłać
                  </button>
                </div>
              )}
            </article>
          );
        })}
      </div>
    </section>
  );
}
