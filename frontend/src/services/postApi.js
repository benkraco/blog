import { apiFetch } from "./api";

export async function getAllPosts() {
    return await apiFetch("/api/posts");
}