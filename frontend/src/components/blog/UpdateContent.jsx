import { useState } from "react";

function UpdateContent({ post, onClose, onUpdated }) {
  const [title, setTitle] = useState(post.title);
  const [markdownFile, setMarkdownFile] = useState(null);

  const [newImages, setNewImages] = useState([]);
  const [deletedImageIds, setDeletedImageIds] = useState([]);

  const [loading, setLoading] = useState(false);

  function handleMarkdownChange(event) {
    const file = event.target.files[0];

    if (!file) {
      return;
    }

    setMarkdownFile(file);
  }

  function handleImagesChange(event) {
    const files = Array.from(event.target.files);

    setNewImages((currentImages) => [...currentImages, ...files]);

    event.target.value = "";
  }

  function handleDeleteExistingImage(imageId) {
    setDeletedImageIds((currentIds) => [...currentIds, imageId]);
  }

  function handleRemoveNewImage(index) {
    setNewImages((currentImages) =>
      currentImages.filter((_, imageIndex) => imageIndex !== index),
    );
  }

  async function handleSubmit(event) {
    event.preventDefault();

    setLoading(true);

    try {
      const formData = new FormData();

      formData.append("Title", title);

      if (markdownFile) {
        formData.append("MarkdownFile", markdownFile);
      }

      newImages.forEach((image) => {
        formData.append("Images", image);
      });

      deletedImageIds.forEach((imageId) => {
        formData.append("DeletedImageIds", imageId);
      });

      const response = await fetch(
        `${import.meta.env.VITE_API_URL}/api/posts/${post.id}`,
        {
          method: "PUT",
          credentials: "include",
          body: formData,
        },
      );

      if (!response.ok) {
        const data = await response.json().catch(() => null);

        throw new Error(data?.message || "No se pudo actualizar el post.");
      }

      const updatedPost = await response.json();
      onUpdated(updatedPost);

      alert("Posteo editado correctamente");
    } catch (error) {
      console.error("Error al actualizar el post:", error);
      alert("ERROR - No se pudo editar el posteo");
    } finally {
      setLoading(false);
    }
  }

  const visibleImages = post.images.filter(
    (image) => !deletedImageIds.includes(image.id),
  );

  return (
    <div className="updateContent">
      <h2>Editar post</h2>

      <form onSubmit={handleSubmit}>
        <div className="updateTitleField">
          <label htmlFor="updateTitle">Título:</label>

          <input
            id="updateTitle"
            type="text"
            value={title}
            onChange={(event) => setTitle(event.target.value)}
            required
          />
        </div>

        <div className="updateTitleField">
          <div className="updateMarkdown">
            <label htmlFor="updateTitle">Archivo nuevo:</label>
            <label htmlFor="updateMarkdown" className="updateFileButton">
              Seleccionar archivo
            </label>

            <input
              id="updateMarkdown"
              className="updateFileInput"
              type="file"
              accept=".md"
              onChange={handleMarkdownChange}
            />

            {markdownFile && (
              <div className="updateSelectedFile">
                <span>{markdownFile.name}</span>

                <span>{(markdownFile.size / 1024).toFixed(2)} KB</span>
              </div>
            )}
          </div>
        </div>

        <div className="updateImages">
          <h3>Imágenes actuales:</h3>

          {visibleImages.length === 0 ? (
            <p>Este post no tiene imágenes.</p>
          ) : (
            <div className="updateImagesList">
              {visibleImages.map((image) => (
                <div className="updateImage" key={image.id}>
                  <img src={image.url} alt={image.alt || image.name} />

                  <button
                    type="button"
                    onClick={() => handleDeleteExistingImage(image.id)}
                  >
                    Eliminar
                  </button>
                </div>
              ))}
            </div>
          )}
        </div>

        <div className="updateTitleField">
          <div className="updateMarkdown">
            <label htmlFor="updateTitle">Imagenes nuevas:</label>
            <label htmlFor="updateImages" className="updateFileButton">
              Seleccionar imágenes
            </label>

            <input
              id="updateImages"
              className="updateFileInput"
              type="file"
              accept="image/*"
              multiple
              onChange={handleImagesChange}
            />
          </div>

          {newImages.length > 0 && (
            <div className="uploadImageList">
              {newImages.map((image, index) => (
                <div className="updateSelectedFile" key={index}>
                  <span>{image.name}</span>

                  <span>{(image.size / 1024).toFixed(2)} KB</span>

                  <button
                    type="button"
                    onClick={() => handleRemoveNewImage(index)}
                  >
                    Quitar
                  </button>
                </div>
              ))}
            </div>
          )}
        </div>

        <div className="postModalActions">
          <button type="button" onClick={onClose} disabled={loading}>
            Cancelar
          </button>

          <button type="submit" disabled={loading}>
            {loading ? "Guardando..." : "Guardar cambios"}
          </button>
        </div>
      </form>
    </div>
  );
}

export default UpdateContent;
