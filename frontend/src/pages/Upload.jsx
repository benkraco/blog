import PageTitle from "../components/ui/PageTitle";
import BlogWindow from "../components/blog/BlogWindow";
import { useState, useEffect } from "react";
import { getAllTags } from "../services/tagApi";

function Upload() {
  const [tags, setTags] = useState([]);
  const [selectedTags, setSelectedTags] = useState([]);
  const [selectedFile, setSelectedFile] = useState(null);
  const [selectedImages, setSelectedImages] = useState([]);

  useEffect(() => {
    async function loadTags() {
      try {
        const data = await getAllTags();

        console.log("TAGS RECIBIDOS:", data);

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

    if (file) {
      setSelectedFile(file);
    }

    if (!file.name.toLowerCase().endsWith(".md")) {
      alert("ERROR - Solo se permiten archivos Markdown (.md)");

      event.target.value = "";
      setSelectedFile(null);

      return;
    }
  }

  function handleImagesChange(event) {
    const files = Array.from(event.target.files);

    setSelectedImages(files);
  }

  return (
    <>
      <PageTitle title="Upload" />
      <BlogWindow title="Upload.exe">
        <form action="" className="formRetro uploadForm">
          <h2>Subir post</h2>

          <div className="uploadInputs">
            <p>Titulo:</p>
            <input type="text" name="" id="" required />
          </div>
          <div className="uploadInputs">
            <p>Dia de creacion:</p>
            <input type="datetime-local" name="" id="" required/>
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
                  <div key={index} className="uploadImageItem">
                    <img src={URL.createObjectURL(image)} alt={image.name} />

                    <div className="uploadImageInfo">
                      <span>{image.name}</span>
                      <span>{image.type}</span>
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
