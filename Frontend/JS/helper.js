// Base URL API (tự động lấy host + port)
const getBaseUrl = () => {
    const url = new URL("https://localhost:7123");
    return `${url.protocol}//${url.host}`;
};

const fullServerUrl = getBaseUrl();

// Convert ảnh relative path → full URL
const getFullImageUrl = (relativeUrl) => {
    if (!relativeUrl) return '';
    if (relativeUrl.startsWith('http://') || relativeUrl.startsWith('https://')) {
        return relativeUrl;
    }
    return `${fullServerUrl}${relativeUrl.startsWith('/') ? '' : '/'}${relativeUrl.trimStart('/')}`;
};