import perspective from "https://cdn.jsdelivr.net/npm/@finos/perspective/dist/cdn/perspective.js";

export async function loadJson(schema, data, view) {
  const worker = await perspective.worker();
  const table = await worker.table(schema);
  table.update(data);
  view.load(table);
}

export async function fetchJson(schema, url, view) {
  const worker = perspective.worker();
  const table = await worker.table(schema);
  let resp = await fetch(url);
  let json = await resp.json();

  table.update(json);
  view.load(table);
}

export async function dispose() {

  if (view) {
    await view.delete();
  }

  if (table) {
    await table.delete();
  }
}

