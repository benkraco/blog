import PageTitle from "../components/ui/PageTitle";
import BlogWindow from "../components/blog/BlogWindow";
import { useState, useEffect } from "react";
import { getAllTags } from "../services/tagApi";

function Upload() {
  const [tags, setTags] = useState([]);
  const [selectedTags, setSelectedTags] = useState([]);
  const [selectedFile, setSelectedFile] = useState(null);

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
  }

  return (
    <>
      <PageTitle title="Upload" />
      <BlogWindow title="Upload.exe">
        <form action="" className="formRetro uploadForm">
          <h2>Subir post</h2>

          <div className="uploadInputs">
            <p>Titulo:</p>
            <input type="text" name="" id="" required/>
          </div>
          <div className="uploadInputs">
            <p>Dia de creacion:</p>
            <input type="datetime-local" name="" id="" />
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
              onChange={handleFileChange}
            />

            <label htmlFor="file" className="uploadFileButton">
              Seleccionar archivo
            </label>

            {selectedFile && (
              <span className="uploadFileName">{selectedFile.name}</span>
            )}
          </div>
          <input type="submit" value="Publicar" />
        </form>
      </BlogWindow>
    </>
  );
}

export default Upload;
