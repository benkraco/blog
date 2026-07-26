import { useParams } from "react-router-dom";
import { useEffect, useState } from "react";
import LoadingMessage from "../ui/Loading";
import ErrorMessage from "../ui/Error";
import PageTitle from "../ui/PageTitle";

function PostContent() {
  const { slug } = useParams();

  const [post, setPost] = useState(null);
  const [loading, setLoading] = useState(true);
  const [error, setError] = useState(null);

  function formatDate(date) {
    return new Date(date).toLocaleString("es-AR", {
      day: "2-digit",
      month: "2-digit",
      year: "numeric",
      hour: "2-digit",
      minute: "2-digit",
      hour12: false,
    });
  }

  useEffect(() => {
    async function loadPost() {
      try {
        const response = await fetch(
          `${import.meta.env.VITE_API_URL}/api/posts/slug/${slug}`,
        );

        if (!response.ok) {
          throw new Error("No se pudo cargar el post");
        }

        const data = await response.json();

        setPost(data);
      } catch (error) {
        console.error(error);
        setError(error.message);
      } finally {
        setLoading(false);
      }
    }

    loadPost();
  }, [slug]);

  if (loading) {
    return <LoadingMessage />;
  }

  if (error) {
    return <ErrorMessage message="El post no se pudo cargar" />;
  }

  if (!post) {
    return <ErrorMessage message="Post no encontrado" />;
  }
  return (
    <>
      <PageTitle title={post.title} />
      <article className="articlePost">
        <h1>{post.title}</h1>
        <div className="postInfoDates">
          <p>Publicado el: {formatDate(post.publishedAt)}</p>
          <p>Creado el: {formatDate(post.createdAt)}</p>
          <p>Última edición: {formatDate(post.updatedAt)}</p>
        </div>
        <div className="postContent">{post.content}</div>
      </article>
    </>
  );
}

export default PostContent;
