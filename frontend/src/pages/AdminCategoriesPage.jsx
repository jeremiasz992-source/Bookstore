import { useEffect, useState } from "react";
import { createCategory, deleteCategory, getCategories, updateCategory } from "../api/categories";
import { Message } from "../components/Message.jsx";

export function AdminCategoriesPage() {
  const [categories, setCategories] = useState([]);
  const [name, setName] = useState("");
  const [editingCategoryId, setEditingCategoryId] = useState(null);
  const [message, setMessage] = useState({ error: "", success: "" });

  async function load() {
    setCategories(await getCategories());
  }

  useEffect(() => {
    load().catch((err) => setMessage({ error: err.message, success: "" }));
  }, []);

  async function handleSubmit(event) {
    event.preventDefault();
    setMessage({ error: "", success: "" });
    try {
      if (editingCategoryId) {
        await updateCategory(editingCategoryId, { name });
      } else {
        await createCategory({ name });
      }
      setName("");
      setEditingCategoryId(null);
      await load();
      setMessage({ error: "", success: editingCategoryId ? "Zapisano kategorię." : "Dodano kategorię." });
    } catch (err) {
      setMessage({ error: err.message, success: "" });
    }
  }

  function startEdit(category) {
    setEditingCategoryId(category.categoryId);
    setName(category.name);
    setMessage({ error: "", success: "" });
  }

  function cancelEdit() {
    setEditingCategoryId(null);
    setName("");
    setMessage({ error: "", success: "" });
  }

  async function handleDelete(categoryId) {
    setMessage({ error: "", success: "" });
    try {
      await deleteCategory(categoryId);
      await load();
      setMessage({ error: "", success: "Usunięto kategorię." });
    } catch (err) {
      setMessage({ error: err.message, success: "" });
    }
  }

  return (
    <section className="panel">
      <h1>Admin: kategorie</h1>
      <form className="inline-form" onSubmit={handleSubmit}>
        <input placeholder="Nazwa kategorii" value={name} onChange={(e) => setName(e.target.value)} />
        <button type="submit">{editingCategoryId ? "Zapisz" : "Dodaj"}</button>
        {editingCategoryId && <button type="button" className="secondary" onClick={cancelEdit}>Anuluj</button>}
      </form>
      <Message error={message.error} success={message.success} />
      <table className="admin-table">
        <thead>
          <tr>
            <th>ID</th>
            <th>Nazwa</th>
            <th>Akcje</th>
          </tr>
        </thead>
        <tbody>
          {categories.map((category) => (
            <tr key={category.categoryId}>
              <td>{category.categoryId}</td>
              <td>{category.name}</td>
              <td className="table-actions">
                <button type="button" className="secondary" onClick={() => startEdit(category)}>Edytuj</button>
                <button type="button" onClick={() => handleDelete(category.categoryId)}>Usuń</button>
              </td>
            </tr>
          ))}
        </tbody>
      </table>
    </section>
  );
}
