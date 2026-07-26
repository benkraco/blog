import { apiFetch } from "./api";

export async function getAllTags() {
    return await apiFetch("/api/tags");
}