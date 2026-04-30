using Microsoft.AspNetCore.Mvc;

namespace blog_api.Controllers;

[ApiController]
public class DocsController : ControllerBase
{
    [HttpGet("/docs")]
    public ContentResult Index()
    {
        return Content(
            """
            <!DOCTYPE html>
            <html lang="en">
            <head>
                <meta charset="utf-8">
                <meta name="viewport" content="width=device-width, initial-scale=1">
                <title>Blog API Docs</title>
                <style>
                    :root {
                        color-scheme: light;
                        --bg: #f4efe8;
                        --panel: #fffaf3;
                        --panel-strong: #ffffff;
                        --border: #d9cbb8;
                        --text: #1f2937;
                        --muted: #6b7280;
                        --accent: #9a3412;
                        --accent-soft: #fed7aa;
                        --ok: #166534;
                    }

                    * { box-sizing: border-box; }

                    body {
                        margin: 0;
                        font-family: "Segoe UI", Arial, sans-serif;
                        background:
                            radial-gradient(circle at top left, #fff7ed 0, transparent 30%),
                            linear-gradient(180deg, #f7f1e8 0%, #efe4d3 100%);
                        color: var(--text);
                    }

                    .page {
                        max-width: 1100px;
                        margin: 0 auto;
                        padding: 32px 20px 48px;
                    }

                    .hero {
                        background: linear-gradient(135deg, #fffaf3 0%, #fde7cf 100%);
                        border: 1px solid var(--border);
                        border-radius: 20px;
                        padding: 28px;
                        box-shadow: 0 18px 50px rgba(31, 41, 55, 0.08);
                    }

                    h1, h2, h3 {
                        margin-top: 0;
                    }

                    h1 {
                        font-size: 2rem;
                        margin-bottom: 8px;
                    }

                    p {
                        line-height: 1.55;
                    }

                    .muted {
                        color: var(--muted);
                    }

                    .layout {
                        display: grid;
                        grid-template-columns: 320px 1fr;
                        gap: 20px;
                        margin-top: 20px;
                    }

                    .card {
                        background: var(--panel);
                        border: 1px solid var(--border);
                        border-radius: 18px;
                        padding: 20px;
                        box-shadow: 0 10px 30px rgba(31, 41, 55, 0.06);
                    }

                    .stack {
                        display: grid;
                        gap: 14px;
                    }

                    label {
                        display: block;
                        font-weight: 600;
                        margin-bottom: 6px;
                    }

                    input, textarea, select, button {
                        width: 100%;
                        font: inherit;
                        border-radius: 12px;
                    }

                    input, textarea, select {
                        border: 1px solid var(--border);
                        background: var(--panel-strong);
                        padding: 12px 14px;
                        color: var(--text);
                    }

                    textarea {
                        min-height: 170px;
                        resize: vertical;
                    }

                    button {
                        border: 0;
                        background: var(--accent);
                        color: white;
                        padding: 12px 16px;
                        font-weight: 700;
                        cursor: pointer;
                    }

                    button.secondary {
                        background: #374151;
                    }

                    button.ghost {
                        background: #ead8c1;
                        color: #7c2d12;
                    }

                    .pill {
                        display: inline-block;
                        padding: 4px 10px;
                        border-radius: 999px;
                        background: var(--accent-soft);
                        color: #9a3412;
                        font-size: 0.8rem;
                        font-weight: 700;
                        margin-bottom: 10px;
                    }

                    .endpoint-list {
                        display: grid;
                        gap: 10px;
                    }

                    .endpoint {
                        border: 1px solid var(--border);
                        border-radius: 14px;
                        padding: 12px;
                        background: #fffdf9;
                    }

                    .method {
                        display: inline-block;
                        min-width: 72px;
                        text-align: center;
                        border-radius: 999px;
                        padding: 4px 8px;
                        font-weight: 700;
                        font-size: 0.8rem;
                        margin-right: 8px;
                        color: white;
                    }

                    .get { background: #2563eb; }
                    .post { background: #059669; }
                    .put { background: #d97706; }
                    .patch { background: #7c3aed; }
                    .delete { background: #dc2626; }

                    pre {
                        margin: 0;
                        white-space: pre-wrap;
                        word-break: break-word;
                        background: #1f2937;
                        color: #f9fafb;
                        padding: 18px;
                        border-radius: 16px;
                        min-height: 280px;
                        overflow: auto;
                    }

                    .response-meta {
                        display: flex;
                        gap: 10px;
                        flex-wrap: wrap;
                        margin-bottom: 10px;
                    }

                    .response-meta span {
                        background: #ecfdf5;
                        color: var(--ok);
                        border-radius: 999px;
                        padding: 6px 10px;
                        font-size: 0.85rem;
                        font-weight: 700;
                    }

                    .hint {
                        font-size: 0.92rem;
                        color: var(--muted);
                    }

                    @media (max-width: 900px) {
                        .layout {
                            grid-template-columns: 1fr;
                        }
                    }
                </style>
            </head>
            <body>
                <div class="page">
                    <section class="hero">
                        <div class="pill">Interactive Docs</div>
                        <h1>Blog API Test Console</h1>
                        <p class="muted">
                            Use this page to register, log in, save your bearer token, and test the blog endpoints directly in the browser.
                        </p>
                    </section>

                    <section class="layout">
                        <aside class="card stack">
                            <div>
                                <h2>Request Builder</h2>
                                <p class="hint">Pick an endpoint preset or edit the fields manually.</p>
                            </div>

                            <div>
                                <label for="token">Bearer Token</label>
                                <input id="token" placeholder="Paste token here after login">
                            </div>

                            <div>
                                <label for="method">Method</label>
                                <select id="method">
                                    <option>GET</option>
                                    <option>POST</option>
                                    <option>PUT</option>
                                    <option>PATCH</option>
                                    <option>DELETE</option>
                                </select>
                            </div>

                            <div>
                                <label for="url">Endpoint</label>
                                <input id="url" value="/api/posts">
                            </div>

                            <div>
                                <label for="body">JSON Body</label>
                                <textarea id="body">{
              "title": "My first blog post",
              "summary": "A short intro post",
              "content": "This is the full content of the post.",
              "tags": ["intro", "blog", "dotnet"],
              "isPublished": true
            }</textarea>
                            </div>

                            <div class="stack">
                                <button id="sendButton" type="button">Send Request</button>
                                <button id="saveTokenButton" class="secondary" type="button">Save Token Locally</button>
                                <button id="clearButton" class="ghost" type="button">Clear Response</button>
                            </div>
                        </aside>

                        <main class="stack">
                            <section class="card">
                                <h2>Quick Start</h2>
                                <div class="endpoint-list">
                                    <div class="endpoint">
                                        <span class="method post">POST</span><strong>/api/auth/register</strong>
                                        <p class="hint">Create a new user.</p>
                                        <button type="button" data-method="POST" data-url="/api/auth/register" data-body='{"username":"grace","password":"password123"}'>Use</button>
                                    </div>
                                    <div class="endpoint">
                                        <span class="method post">POST</span><strong>/api/auth/login</strong>
                                        <p class="hint">Get a token for an existing user.</p>
                                        <button type="button" data-method="POST" data-url="/api/auth/login" data-body='{"username":"grace","password":"password123"}'>Use</button>
                                    </div>
                                    <div class="endpoint">
                                        <span class="method get">GET</span><strong>/api/auth/me</strong>
                                        <p class="hint">Check the currently authenticated user.</p>
                                        <button type="button" data-method="GET" data-url="/api/auth/me" data-body="">Use</button>
                                    </div>
                                    <div class="endpoint">
                                        <span class="method get">GET</span><strong>/api/posts</strong>
                                        <p class="hint">List published posts.</p>
                                        <button type="button" data-method="GET" data-url="/api/posts" data-body="">Use</button>
                                    </div>
                                    <div class="endpoint">
                                        <span class="method post">POST</span><strong>/api/posts</strong>
                                        <p class="hint">Create a post as the logged-in user.</p>
                                        <button type="button" data-method="POST" data-url="/api/posts" data-body='{"title":"My first blog post","summary":"A short intro post","content":"This is the full content of the post.","tags":["intro","blog","dotnet"],"isPublished":true}'>Use</button>
                                    </div>
                                    <div class="endpoint">
                                        <span class="method post">POST</span><strong>/api/posts/1/comments</strong>
                                        <p class="hint">Add a comment as the logged-in user.</p>
                                        <button type="button" data-method="POST" data-url="/api/posts/1/comments" data-body='{"message":"Nice post."}'>Use</button>
                                    </div>
                                </div>
                            </section>

                            <section class="card">
                                <h2>Response</h2>
                                <div class="response-meta">
                                    <span id="statusBadge">Status: waiting</span>
                                    <span id="urlBadge">URL: -</span>
                                </div>
                                <pre id="responseOutput">Run a request to see the response here.</pre>
                            </section>
                        </main>
                    </section>
                </div>

                <script>
                    const tokenInput = document.getElementById("token");
                    const methodInput = document.getElementById("method");
                    const urlInput = document.getElementById("url");
                    const bodyInput = document.getElementById("body");
                    const responseOutput = document.getElementById("responseOutput");
                    const statusBadge = document.getElementById("statusBadge");
                    const urlBadge = document.getElementById("urlBadge");

                    const savedToken = localStorage.getItem("blogApiToken");
                    if (savedToken) {
                        tokenInput.value = savedToken;
                    }

                    document.querySelectorAll("[data-method]").forEach((button) => {
                        button.addEventListener("click", () => {
                            methodInput.value = button.dataset.method;
                            urlInput.value = button.dataset.url;
                            bodyInput.value = button.dataset.body || "";
                        });
                    });

                    document.getElementById("saveTokenButton").addEventListener("click", () => {
                        localStorage.setItem("blogApiToken", tokenInput.value.trim());
                        statusBadge.textContent = "Status: token saved";
                    });

                    document.getElementById("clearButton").addEventListener("click", () => {
                        responseOutput.textContent = "Run a request to see the response here.";
                        statusBadge.textContent = "Status: waiting";
                        urlBadge.textContent = "URL: -";
                    });

                    document.getElementById("sendButton").addEventListener("click", async () => {
                        const method = methodInput.value;
                        const path = urlInput.value.trim();
                        const token = tokenInput.value.trim();
                        const headers = {};
                        const options = { method, headers };

                        if (token) {
                            headers["Authorization"] = `Bearer ${token}`;
                        }

                        if (method !== "GET" && method !== "DELETE") {
                            headers["Content-Type"] = "application/json";

                            if (bodyInput.value.trim()) {
                                try {
                                    JSON.parse(bodyInput.value);
                                } catch (error) {
                                    statusBadge.textContent = "Status: invalid JSON body";
                                    responseOutput.textContent = error.toString();
                                    return;
                                }

                                options.body = bodyInput.value;
                            }
                        }

                        try {
                            const response = await fetch(path, options);
                            const text = await response.text();
                            let formatted = text;

                            try {
                                formatted = JSON.stringify(JSON.parse(text), null, 2);
                            } catch {
                            }

                            statusBadge.textContent = `Status: ${response.status} ${response.statusText}`;
                            urlBadge.textContent = `URL: ${path}`;
                            responseOutput.textContent = formatted || "(empty response)";

                            if (path === "/api/auth/login" || path === "/api/auth/register") {
                                try {
                                    const payload = JSON.parse(text);
                                    if (payload.token) {
                                        tokenInput.value = payload.token;
                                        localStorage.setItem("blogApiToken", payload.token);
                                    }
                                } catch {
                                }
                            }
                        } catch (error) {
                            statusBadge.textContent = "Status: request failed";
                            urlBadge.textContent = `URL: ${path}`;
                            responseOutput.textContent = error.toString();
                        }
                    });
                </script>
            </body>
            </html>
            """,
            "text/html");
    }
}
