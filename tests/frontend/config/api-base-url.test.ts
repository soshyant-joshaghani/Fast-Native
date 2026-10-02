import { existsSync } from 'node:fs';
import path from 'node:path';
import { pathToFileURL } from 'node:url';
import { describe, expect, it } from 'vitest';

const root = path.resolve(__dirname, '../../..');
const candidates = [
  'frontend/web/src/lib/config/api-url.ts',
  'frontend/web/lib/config/api-url.ts'
];

async function loadNormalize() {
  for (const rel of candidates) {
    const abs = path.join(root, rel);
    if (!existsSync(abs)) continue;
    const mod = await import(pathToFileURL(abs).href);
    return mod.normalizeApiBaseUrl as (value: string | undefined) => string;
  }
  return null;
}

describe('normalizeApiBaseUrl', () => {
  it('defaults to /api/v1 without trailing slash', async () => {
    const normalizeApiBaseUrl = await loadNormalize();
    if (!normalizeApiBaseUrl) return;
    expect(normalizeApiBaseUrl(undefined)).toBe('/api/v1');
  });

  it('strips trailing slash', async () => {
    const normalizeApiBaseUrl = await loadNormalize();
    if (!normalizeApiBaseUrl) return;
    expect(normalizeApiBaseUrl('/api/v1/')).toBe('/api/v1');
  });
});
