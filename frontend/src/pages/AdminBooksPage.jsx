import { useEffect, useState } from "react";
import { createBook, deleteBook, getBooks, updateBook } from "../api/books";
import { getCategories } from "../api/categories";
import { Message } from "../components/Message.jsx";
import { formatPrice } from "../labels.js";

const emptyBook = {
  title: "",
  author: "",
  publisher: "",
  description: "",
  coverImageUrl: "",
  year: new Date().getFullYear(),
  price: 1,
  stock: 1,
  categoryId: 1
};

export function AdminBooksPage() {
  const [books, setBooks] = useState([]);
  const [categories, setCategories] = useState([]);
  const [form, setForm] = useState(emptyBook);
  const [editingBookId, setEditingBookId] = useState(null);
  const [message, setMessage] = useState({ error: "", success: "" });

  async function load() {
    const [bookPage, categoryList] = await Promise.all([getBooks({ pageSize: 100 }), getCategories()]);
    setBooks(bookPage.items);
    setCategories(categoryList);
  }

  useEffect(() => {
    load().catch((err) => setMessage({ error: err.message, success: "" }));
  }, []);

  async function handleSubmit(event) {
    event.preventDefault();
    setMessage({ error: "", success: "" });
    const payload = {
      ...form,
      year: Number(form.year),
      price: Number(form.price),
      stock: Number(form.stock),
      categoryId: Number(form.categoryId)
    };

    try {
      if (editingBookId) {
        await updateBook(editingBookId, payload);
      } else {
        await createBook(payload);
      }
      setForm(emptyBook);
      setEditingBookId(null);
      await load();
      setMessage({ error: "", success: editingBookId ? "Zapisano książkę." : "Dodano książkę." });
    } catch (err) {
      setMessage({ error: err.message, success: "" });
    }
  }

  function startEdit(book) {
    setEditingBookId(book.bookId);
    setForm({
      title: book.title,
      author: book.author,
      publisher: book.publisher,
      description: book.description || "",
      coverImageUrl: book.coverImageUrl || "",
      year: book.year,
      price: book.price,
      stock: book.stock,
      categoryId: book.categoryId
    });
    setMessage({ error: "", success: "" });
  }

  function cancelEdit() {
    setEditingBookId(null);
    setForm(emptyBook);
    setMessage({ error: "", success: "" });
  }

  async function handleDelete(bookId) {
    setMessage({ error: "", success: "" });
    try {
      await deleteBook(bookId);
      await load();
      setMessage({ error: "", success: "Usunięto książkę." });
    } catch (err) {
      setMessage({ error: err.message, success: "" });
    }
  }

  return (
    <section className="admin-grid">
      <div className="panel">
        <h1>Admin: książki</h1>
        <form className="form" onSubmit={handleSubmit}>
          <input placeholder="Tytuł" value={form.title} onChange={(e) => setForm({ ...form, title: e.target.value })} />
          <input placeholder="Autor" value={form.author} onChange={(e) => setForm({ ...form, author: e.target.value })} />
          <input placeholder="Wydawca" value={form.publisher} onChange={(e) => setForm({ ...form, publisher: e.target.value })} />
          <input placeholder="Ścieżka okładki (np. /covers/default-book.svg)" value={form.coverImageUrl} onChange={(e) => setForm({ ...form, coverImageUrl: e.target.value })} />
          <textarea placeholder="Opis książki" rows={4} value={form.description} onChange={(e) => setForm({ ...form, description: e.target.value })} />
          <input type="number" value={form.year} onChange={(e) => setForm({ ...form, year: e.target.value })} />
          <input type="number" step="0.01" value={form.price} onChange={(e) => setForm({ ...form, price: e.target.value })} />
          <input type="number" value={form.stock} onChange={(e) => setForm({ ...form, stock: e.target.value })} />
          <select value={form.categoryId} onChange={(e) => setForm({ ...form, categoryId: e.target.value })}>
            {categories.map((category) => (
              <option key={category.categoryId} value={category.categoryId}>{category.name}</option>
            ))}
          </select>
          <button type="submit">{editingBookId ? "Zapisz książkę" : "Dodaj książkę"}</button>
          {editingBookId && <button type="button" className="secondary" onClick={cancelEdit}>Anuluj edycję</button>}
        </form>
        <Message error={message.error} success={message.success} />
      </div>
      <div className="panel">
        <h2>Lista</h2>
        <table className="admin-table">
          <thead>
            <tr>
              <th>Tytuł</th>
              <th>Kategoria</th>
              <th>Cena</th>
              <th>Stan</th>
              <th>Akcje</th>
            </tr>
          </thead>
          <tbody>
            {books.map((book) => (
              <tr key={book.bookId}>
                <td>{book.title}</td>
                <td>{book.categoryName}</td>
                <td>{formatPrice(book.price)} PLN</td>
                <td>{book.stock}</td>
                <td className="table-actions">
                  <button type="button" className="secondary" onClick={() => startEdit(book)}>Edytuj</button>
                  <button type="button" onClick={() => handleDelete(book.bookId)}>Usuń</button>
                </td>
              </tr>
            ))}
          </tbody>
        </table>
      </div>
    </section>
  );
}
