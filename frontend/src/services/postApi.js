import { apiFetch } from "./api";

export async function getAllPosts() {
    return await apiFetch("/api/posts");
}

export async function createPost({
  title,
  createdAt,
  markdownFile,
  images,
}) {
  const formData = new FormData();

  formData.append("Title", title);
  formData.append("CreatedAt", createdAt);
  formData.append("MarkdownFile", markdownFile);

  images.forEach((image) => {
    formData.append("Images", image);
  });

  const response = await apiFetch("/api/posts", {
    method: "POST",
    body: formData,
  });

  return response;
}