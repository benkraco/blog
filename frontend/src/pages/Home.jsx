import { useEffect, useState } from "react";
import { getAllPosts } from "../services/postApi";

function App() {
  const [posts, setPosts] = useState([]);
  const [loading, setLoading] = useState(true);
  const [error, setError] = useState(null);

  useEffect(() => {
    async function loadPosts() {
      try {
        const data = await getAllPosts();
        setPosts(data);
      } catch (error) {
        setError("No se pudieron cargar los posts");
      } finally {
        setLoading(false);
      }
    }

    loadPosts();
  }, []);

  if (loading) {
    return <p>Cargando posts...</p>;
  }

  if (error) {
    return <p>{error}</p>;
  }
  return (
    <div>
      <h1>Home</h1>
      {posts.map((post) => (
        <article key={post.id}>
          <h2>{post.title}</h2>
          <p>{post.slug}</p>
        </article>
      ))}
    </div>
  );
}

export default App;
