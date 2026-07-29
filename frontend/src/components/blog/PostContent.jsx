import { useParams, useNavigate } from "react-router-dom";
import { useEffect, useState } from "react";
import LoadingMessage from "../ui/Loading";
import ErrorMessage from "../ui/Error";
import PageTitle from "../ui/PageTitle";
import useAuth from "../../hooks/useAuth";

function PostContent() {
  const { slug } = useParams();
  const navigate = useNavigate();
  const { isAuthenticated } = useAuth();

  const [post, setPost] = useState(null);
  const [loading, setLoading] = useState(true);
  const [error, setError] = useState(null);

  const [showEditModal, setShowEditModal] = useState(false);
  const [showDeleteModal, setShowDeleteModal] = useState(false);

  const [selectedImageIndex, setSelectedImageIndex] = useState(null);

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

  function openGallery(index) {
    setSelectedImageIndex(index);
  }

  function closeGallery() {
    setSelectedImageIndex(null);
  }

  function showPreviousImage(event) {
    event.stopPropagation();

    setSelectedImageIndex((currentIndex) => {
      if (currentIndex === 0) {
        return post.images.length - 1;
      }

      return currentIndex - 1;
    });
  }

  function showNextImage(event) {
    event.stopPropagation();

    setSelectedImageIndex((currentIndex) => {
      if (currentIndex === post.images.length - 1) {
        return 0;
      }

      return currentIndex + 1;
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

  useEffect(() => {
    function handleKeyboard(event) {
      if (selectedImageIndex === null) {
        return;
      }

      if (event.key === "Escape") {
        closeGallery();
      }

      if (event.key === "ArrowLeft") {
        setSelectedImageIndex((currentIndex) => {
          if (currentIndex === 0) {
            return post.images.length - 1;
          }

          return currentIndex - 1;
        });
      }

      if (event.key === "ArrowRight") {
        setSelectedImageIndex((currentIndex) => {
          if (currentIndex === post.images.length - 1) {
            return 0;
          }

          return currentIndex + 1;
        });
      }
    }

    document.addEventListener("keydown", handleKeyboard);

    return () => {
      document.removeEventListener("keydown", handleKeyboard);
    };
  }, [selectedImageIndex, post]);

  async function handleDelete() {
    try {
      const response = await fetch(
        `${import.meta.env.VITE_API_URL}/api/posts/${post.id}`,
        {
          method: "DELETE",
          credentials: "include",
        },
      );

      if (!response.ok) {
        throw new Error("No se pudo borrar el post");
      }

      alert("Post borrado correctamente.");

      navigate("/");
    } catch (error) {
      console.error("Error al borrar el post:", error);

      alert("No se pudo borrar el post.");
    }
  }

  if (loading) {
    return <LoadingMessage />;
  }

  if (error) {
    return <ErrorMessage message="El post no se pudo cargar" />;
  }

  if (!post) {
    return <ErrorMessage message="Post no encontrado" />;
  }

  function renderContent(content, images) {
    const parts = content.split(/(\[\[image:\d+\]\])/g);

    return parts.map((part, index) => {
      const imageMatch = part.match(/^\[\[image:(\d+)\]\]$/);

      if (imageMatch) {
        const imageIndex = Number(imageMatch[1]);
        const image = images?.[imageIndex];

        if (!image) {
          return null;
        }

        return (
          <div className="postImage" key={index}>
            <img
              src={image.url}
              alt={image.alt || image.name}
              onClick={() => openGallery(imageIndex)}
            />
          </div>
        );
      }

      return <p key={index}>{part}</p>;
    });
  }

  const selectedImage =
    selectedImageIndex !== null ? post.images[selectedImageIndex] : null;

  function formatFileSize(bytes) {
    if (bytes < 1024) {
      return `${bytes} B`;
    }

    if (bytes < 1024 * 1024) {
      return `${(bytes / 1024).toFixed(2)} KB`;
    }

    return `${(bytes / (1024 * 1024)).toFixed(2)} MB`;
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

        <div className="postContent">
          {renderContent(post.content, post.images)}
        </div>
      </article>

      {isAuthenticated && (
        <div className="postActions">
          <button onClick={() => setShowEditModal(true)}>Editar</button>
          <button onClick={() => setShowDeleteModal(true)}>Borrar</button>
        </div>
      )}

      {selectedImage && (
        <div className="imageGalleryOverlay" onClick={closeGallery}>
          <div
            className="imageGallery"
            onClick={(event) => event.stopPropagation()}
          >
            <button className="imageGalleryClose" onClick={closeGallery}>
              ×
            </button>

            <button
              className="imageGalleryPrevious"
              onClick={showPreviousImage}
            >
              ‹
            </button>

            <div className="imageGalleryContent">
              <p>
                {selectedImageIndex + 1} / {post.images.length}
              </p>

              <img
                src={selectedImage.url}
                alt={selectedImage.alt || selectedImage.name}
                className="imageGalleryImage"
              />

              <div className="imageGalleryInfo">
                <h3>{selectedImage.name}</h3>

                {selectedImage.description && (
                  <p>{selectedImage.description}</p>
                )}

                <p>
                  {selectedImage.width} × {selectedImage.height}px
                </p>

                <p>{selectedImage.mimeType}px</p>

                <span>{formatFileSize(selectedImage.fileSize)}</span>
              </div>
            </div>

            <button className="imageGalleryNext" onClick={showNextImage}>
              ›
            </button>
          </div>
        </div>
      )}

      {showDeleteModal && (
        <div className="postModalOverlay">
          <div className="postModal">
            <h2>¿Estás seguro?</h2>

            <p>¿Querés borrar el post "{post.title}"?</p>

            <div className="postModalActions">
              <button onClick={() => setShowDeleteModal(false)}>No</button>

              <button onClick={handleDelete}>Sí</button>
            </div>
          </div>
        </div>
      )}
    </>
  );
}

export default PostContent;
