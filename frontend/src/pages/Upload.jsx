import PageTitle from "../components/ui/PageTitle";
import BlogWindow from "../components/blog/BlogWindow";
import { useState, useEffect } from "react";
import { getAllTags } from "../services/tagApi";
import { createPost } from "../services/postApi";
import { useNavigate } from "react-router-dom";
import flechaAbajo from "../assets/Icons/down-long-solid.png";
import flechaArriba from "../assets/Icons/up-long-solid.png";

function Upload() {
  const [tags, setTags] = useState([]);
  const [selectedTags, setSelectedTags] = useState([]);
  const [selectedFile, setSelectedFile] = useState(null);
  const [selectedImages, setSelectedImages] = useState([]);

  const [title, setTitle] = useState("");
  const [createdAt, setCreatedAt] = useState("");

  const navigate = useNavigate();

  useEffect(() => {
    async function loadTags() {
      try {
        const data = await getAllTags();

        setTags(data);
      } catch (error) {
        console.error("Error al cargar los tags:", error);
      }
    }

    loadTags();
  }, []);

  function handleTagChange(tagId) {
    setSelectedTags((currentTags) => {
      if (currentTags.includes(tagId)) {
        return currentTags.filter((id) => id !== tagId);
      }

      return [...currentTags, tagId];
    });
  }

  function handleFileChange(event) {
    const file = event.target.files[0];

    if (!file) {
      setSelectedFile(null);
      return;
    }

    if (!file.name.toLowerCase().endsWith(".md")) {
      alert("ERROR - Solo se permiten archivos Markdown (.md)");

      event.target.value = "";
      setSelectedFile(null);

      return;
    }

    setSelectedFile(file);
  }

  function handleImagesChange(event) {
    const files = Array.from(event.target.files);

    setSelectedImages(files);

    event.target.value = "";
  }

  function moveImageUp(index) {
    if (index === 0) {
      return;
    }

    setSelectedImages((currentImages) => {
      const newImages = [...currentImages];

      [newImages[index - 1], newImages[index]] = [
        newImages[index],
        newImages[index - 1],
      ];

      return newImages;
    });
  }

  function moveImageDown(index) {
    if (index === selectedImages.length - 1) {
      return;
    }

    setSelectedImages((currentImages) => {
      const newImages = [...currentImages];

      [newImages[index], newImages[index + 1]] = [
        newImages[index + 1],
        newImages[index],
      ];

      return newImages;
    });
  }

  async function handleSubmit(event) {
    event.preventDefault();

    if (!selectedFile) {
      alert("Debes seleccionar un archivo Markdown.");
      return;
    }

    try {
      const post = await createPost({
        title,
        createdAt,
        markdownFile: selectedFile,
        images: selectedImages,
      });

      alert("Post publicado correctamente.");
      navigate(`/post/${post.slug}`);
    } catch (error) {
      console.error("Error al publicar el post:", error);

      alert("No se pudo publicar el post.");
    }
  }

  return (
    <>
      <PageTitle title="Upload" />

      <BlogWindow title="Upload.exe">
        <form className="formRetro uploadForm" onSubmit={handleSubmit}>
          <h2>Subir post</h2>

          <div className="uploadInputs">
            <p>Titulo:</p>

            <input
              type="text"
              value={title}
              onChange={(event) => setTitle(event.target.value)}
              required
            />
          </div>

          <div className="uploadInputs">
            <p>Dia de creacion:</p>

            <input
              type="datetime-local"
              value={createdAt}
              onChange={(event) => setCreatedAt(event.target.value)}
              required
            />
          </div>

          {/*<div className="formField">
            <p>Tags:</p>

            <div className="tagList">
              {tags.map((tag) => (
                <label key={tag.id}>
                  <input
                    type="checkbox"
                    value={tag.id}
                    checked={selectedTags.includes(tag.id)}
                    onChange={() => handleTagChange(tag.id)}
                  />

                  {tag.name}
                </label>
              ))}
            </div>
          </div>*/}

          <div className="uploadFile">
            <input
              type="file"
              id="file"
              className="uploadFileInput"
              accept=".md"
              required
              onChange={handleFileChange}
            />

            <label htmlFor="file" className="uploadFileButton">
              Seleccionar archivo
            </label>

            {selectedFile && (
              <div className="uploadSelectedFile">
                <span>{selectedFile.name}</span>

                <span>{selectedFile.type || "text/markdown"}</span>
              </div>
            )}
          </div>

          <div className="uploadFile">
            <input
              type="file"
              id="images"
              className="uploadFileInput"
              accept="image/*"
              multiple
              onChange={handleImagesChange}
            />

            <label htmlFor="images" className="uploadFileButton">
              Seleccionar imágenes
            </label>

            {selectedImages.length > 0 && (
              <div className="uploadImageList">
                {selectedImages.map((image, index) => (
                  <div
                    key={`${image.name}-${image.lastModified}`}
                    className="uploadImageItem"
                  >
                    <img src={URL.createObjectURL(image)} alt={image.name} />

                    <div className="uploadImageInfo">
                      <span>
                        #{index} — {image.name}
                      </span>

                      <span>{image.type}</span>
                    </div>

                    <div className="uploadImageActions">
                      <button
                        type="button"
                        onClick={() => moveImageUp(index)}
                        disabled={index === 0}
                      >
                        <img src={flechaArriba} alt="Flecha arriba" />
                      </button>

                      <button
                        type="button"
                        onClick={() => moveImageDown(index)}
                        disabled={index === selectedImages.length - 1}
                      >
                        <img src={flechaAbajo} alt="Flecha arriba" />
                      </button>
                    </div>
                  </div>
                ))}
              </div>
            )}
          </div>

          <input type="submit" value="Publicar" />
        </form>
      </BlogWindow>
    </>
  );
}

export default Upload;
