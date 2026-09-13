import { Link, useLocation } from "react-router-dom";
import { describeOrderStatus, formatPrice } from "../labels.js";

export function OrderConfirmationPage() {
  const { state } = useLocation();
  const order = state?.order;

  if (!order) {
    return (
      <section className="panel narrow">
        <h1>Brak danych zamówienia</h1>
        <p>Nie znaleziono szczegółów ostatniego zamówienia.</p>
        <div className="actions">
          <Link className="button-link" to="/orders">Przejdź do zamówień</Link>
          <Link className="button-link secondary" to="/books">Wróć do książek</Link>
        </div>
      </section>
    );
  }

  const total = order.items.reduce((sum, item) => sum + item.price * item.quantity, 0);

  return (
    <section className="panel confirmation">
      <h1>Zamówienie zostało złożone</h1>
      <p className="lead">Numer zamówienia: <strong>#{order.orderId}</strong></p>
      <p>Status: {describeOrderStatus(order.status)}</p>
      {order.confirmationEmailSent === false ? (
        <div className="message error">
          {order.confirmationMessage ||
            "Zamówienie zostało złożone, ale nie udało się wysłać potwierdzenia e-mail."}
        </div>
      ) : (
        <div className="message success">
          Potwierdzenie zamówienia zostało wysłane na Twój adres e-mail.
        </div>
      )}

      <h2>Podsumowanie</h2>
      <div className="list">
        {order.items.map((item) => (
          <div className="row-item order-summary-row" key={`${order.orderId}-${item.bookId}`}>
            <span>{item.title}</span>
            <span>{item.quantity} szt.</span>
            <span>{formatPrice(item.price * item.quantity)} PLN</span>
          </div>
        ))}
      </div>

      <div className="actions">
        <strong>Razem: {formatPrice(total)} PLN</strong>
        <Link className="button-link" to="/orders">Zobacz zamówienia</Link>
        <Link className="button-link secondary" to="/books">Kontynuuj zakupy</Link>
      </div>
    </section>
  );
}
