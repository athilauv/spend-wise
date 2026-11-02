
const configuredApiUrl = process.env.NEXT_PUBLIC_API_BASE_URL;

export const API_BASE_URL = (
    configuredApiUrl || "https://localhost:7264"
).replace(/\/+$/, "");

export const APP_NAME = "SpendWise";
