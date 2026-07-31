import { useEffect, useState } from "react";
import flechaAbajo from "../../assets/Icons/down-long-solid.png";
import flechaArriba from "../../assets/Icons/up-long-solid.png";
import { apiFetch } from "../../services/api";

function UpdateContent({ post, onClose, onUpdated }) {
  const [title, setTitle] = useState(post.title);
  const [markdownFile, setMarkdownFile] = useState(null);

  const [images, setImages] = useState([]);
  const [deletedImageIds, setDeletedImageIds] = useState([]);

  const [loading, setLoading] = useState(false);

  useEffect(() => {
    const existingImages = post.images.map((image) => ({
      type: "existing",
      id: image.id,
      url: image.url,
      name: image.name,
      alt: image.alt,
    }));

    setImages(existingImages);
    setDeletedImageIds([]);
  }, [post]);

  function handleMarkdownChange(event) {
    const file = event.target.files[0];

    if (!file) {
      setMarkdownFile(null);
      return;
    }

    if (!file.name.toLowerCase().endsWith(".md")) {
      alert("ERROR - Solo se permiten archivos Markdown (.md)");

      event.target.value = "";
      setMarkdownFile(null);

      return;
    }

    setMarkdownFile(file);
  }

  function handleImagesChange(event) {
    const files = Array.from(event.target.files);

    const newImageItems = files.map((file) => ({
      type: "new",
      id: crypto.randomUUID(),
      file,
      url: URL.createObjectURL(file),
      name: file.name,
    }));

    setImages((currentImages) => [...currentImages, ...newImageItems]);

    event.target.value = "";
  }

  function moveImageUp(index) {
    if (index === 0) {
      return;
    }

    setImages((currentImages) => {
      const newImages = [...currentImages];

      [newImages[index - 1], newImages[index]] = [
        newImages[index],
        newImages[index - 1],
      ];

      return newImages;
    });
  }

  function moveImageDown(index) {
    if (index === images.length - 1) {
      return;
    }

    setImages((currentImages) => {
      const newImages = [...currentImages];

      [newImages[index], newImages[index + 1]] = [
        newImages[index + 1],
        newImages[index],
      ];

      return newImages;
    });
  }

  function handleDeleteExistingImage(imageId) {
    setDeletedImageIds((currentIds) => [...currentIds, imageId]);

    setImages((currentImages) =>
      currentImages.filter(
        (image) => !(image.type === "existing" && image.id === imageId),
      ),
    );
  }

  function handleRemoveNewImage(imageId) {
    setImages((currentImages) =>
      currentImages.filter(
        (image) => !(image.type === "new" && image.id === imageId),
      ),
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

      const newImages = images.filter((image) => image.type === "new");

      newImages.forEach((image) => {
        formData.append("Images", image.file);
      });

      deletedImageIds.forEach((imageId) => {
        formData.append("DeletedImageIds", imageId);
      });

      images.forEach((image) => {
        if (image.type === "existing") {
          formData.append("ImageOrder", `existing:${image.id}`);
        }

        if (image.type === "new") {
          const newImageIndex = newImages.indexOf(image);

          formData.append("ImageOrder", `new:${newImageIndex}`);
        }
      });

      const updatedPost = await apiFetch(`/api/posts/${post.id}`, {
        method: "PUT",
        body: formData,
      });

      onUpdated(updatedPost);

      alert("Posteo editado correctamente");
    } catch (error) {
      console.error("Error al actualizar el post:", error);

      alert(error.message || "ERROR - No se pudo editar el posteo");
    } finally {
      setLoading(false);
    }
  }

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
            <label htmlFor="updateMarkdown">Archivo nuevo:</label>

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
          <h3>Imágenes del post:</h3>

          {images.length === 0 ? (
            <p>Este post no tiene imágenes.</p>
          ) : (
            <div className="uploadImageList">
              {images.map((image, index) => (
                <div key={image.id} className="uploadImageItem">
                  <img src={image.url} alt={image.alt || image.name} />

                  <div className="uploadImageInfo">
                    <span>
                      #{index} — {image.name}
                    </span>

                    <span>
                      {image.type === "existing"
                        ? "Imagen actual"
                        : image.file.type}
                    </span>
                  </div>

                  <div className="uploadImageActions">
                    <button
                      type="button"
                      onClick={() => moveImageUp(index)}
                      disabled={index === 0}
                    >
                      <img src={flechaArriba} alt="Mover arriba" />
                    </button>

                    <button
                      type="button"
                      onClick={() => moveImageDown(index)}
                      disabled={index === images.length - 1}
                    >
                      <img src={flechaAbajo} alt="Mover abajo" />
                    </button>
                  </div>

                  <button
                    type="button"
                    onClick={() =>
                      image.type === "existing"
                        ? handleDeleteExistingImage(image.id)
                        : handleRemoveNewImage(image.id)
                    }
                    className="updateButtonDeleteImgs"
                  >
                    {image.type === "existing" ? "Eliminar" : "Quitar"}
                  </button>
                </div>
              ))}
            </div>
          )}
        </div>

        <div className="updateTitleField">
          <div className="updateMarkdown">
            <label htmlFor="updateImages">Imágenes nuevas:</label>

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
