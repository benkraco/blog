import { useEffect, useState } from "react";
import { getAllPosts } from "../../services/postApi";
import LoadingMessage from "../ui/Loading";
import ErrorMessage from "../ui/Error";

function PostList() {
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
    return <LoadingMessage />
  }

  if (error) {
    return <ErrorMessage message={error} />
  }
  return (
    <>
      {posts.map((post) => (
        <article key={post.id} className="articlePost">
          <h2>{post.title}</h2>
          <p>
            {new Date(post.publishedAt).toLocaleString("es-AR", {
              day: "2-digit",
              month: "2-digit",
              year: "numeric",
              hour: "2-digit",
              minute: "2-digit",
              hour12: false,
            })}
          </p>
        </article>
      ))}
    </>
  );
}

export default PostList;
