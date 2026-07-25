import { apiFetch } from "./api";

export async function login(username, password) {
    await apiFetch("/api/auth/login", {
        method: "POST",
        body: JSON.stringify({
            username,
            password,
        }),
    });
}

export async function logout() {
    await apiFetch("/api/auth/logout", {
        method: "POST",
    });
}