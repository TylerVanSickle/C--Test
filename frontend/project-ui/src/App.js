// src/App.js
import React, { useEffect, useState } from "react";

function App() {
  const [projects, setProjects] = useState([]);
  const [form, setForm] = useState({
    title: "",
    description: "",
    role: "",
    launchYear: "",
  });

  // Load projects on mount
  useEffect(() => {
    fetch("http://localhost:5163/api/projects") // adjust port if needed
      .then((res) => res.json())
      .then((data) => setProjects(data))
      .catch((err) => console.error("Failed to fetch projects", err));
  }, []);

  // Handle form change
  const handleChange = (e) => {
    setForm({ ...form, [e.target.name]: e.target.value });
  };

  // Handle form submit
  const handleSubmit = (e) => {
    e.preventDefault();
    fetch("http://localhost:5163/api/projects", {
      method: "POST",
      headers: { "Content-Type": "application/json" },
      body: JSON.stringify({ ...form, launchYear: parseInt(form.launchYear) }),
    })
      .then((res) => res.json())
      .then((data) => {
        setProjects((prev) => [...prev, data]);
        setForm({ title: "", description: "", role: "", launchYear: "" });
      });
  };

  return (
    <div style={{ padding: "2rem" }}>
      <h1>Projects</h1>
      <ul>
        {projects.map((p) => (
          <li key={p.id}>
            <strong>{p.title}</strong> - {p.description} ({p.role},{" "}
            {p.launchYear})
          </li>
        ))}
      </ul>

      <h2>Add a Project</h2>
      <form onSubmit={handleSubmit}>
        <input
          name="title"
          placeholder="Title"
          value={form.title}
          onChange={handleChange}
          required
        />
        <br />
        <input
          name="description"
          placeholder="Description"
          value={form.description}
          onChange={handleChange}
          required
        />
        <br />
        <input
          name="role"
          placeholder="Role"
          value={form.role}
          onChange={handleChange}
          required
        />
        <br />
        <input
          name="launchYear"
          placeholder="Launch Year"
          value={form.launchYear}
          onChange={handleChange}
          required
          type="number"
        />
        <br />
        <button type="submit">Add Project</button>
      </form>
    </div>
  );
}

export default App;
