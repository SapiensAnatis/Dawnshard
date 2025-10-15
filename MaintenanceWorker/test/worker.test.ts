import { SELF } from "cloudflare:test";
import { decode, decodeAsync } from "@msgpack/msgpack";

describe("web", () => {
  test("renders webpage", async () => {
    const res = await SELF.fetch("http://localhost");

    expect(res.ok).toBe(true);
    expect(await res.text()).toMatchSnapshot();
  });

  test("renders webpage on any other path", async () => {
    const res = await SELF.fetch("http://localhost/aaaaaa");

    expect(res.ok).toBe(true);
    expect(await res.text()).toMatch(/^<!DOCTYPE html>/);
  });
});

describe("game", () => {
  const deserialize = async (blob: Blob) => {
    if (blob.stream) {
      return await decodeAsync(blob.stream());
    } else {
      return decode(await blob.arrayBuffer());
    }
  };

  test("sends maintenance response", async () => {
    const res = await SELF.fetch(
      "http://localhost/2.19.0_20220714193707/dungeon_start/start",
      {
        method: "POST",
      }
    );

    expect(res.ok).toBe(true);
    const decoded = await deserialize(await res.blob());

    expect(decoded).toEqual({
      data_headers: {
        result_code: 101,
      },
      data: {
        result_code: 101,
      },
    });
  });

  test("sends maintenance text", async () => {
    const res = await SELF.fetch(
      "http://localhost/2.19.0_20220719103923/maintenance/get_text",
      {
        method: "POST",
      }
    );

    expect(res.ok).toBe(true);
    const decoded = await deserialize(await res.blob());

    expect(decoded).toEqual({
      data_headers: {
        result_code: 1,
      },
      data: {
        maintenance_text: `<title>Maintenance</title>
<body>Dawnshard is currently under maintenance so
dreadfullydistinct can fix his
Consul install.</body>
<schedule>Check back at:</schedule>
<date>2024-05-21T06:00:00</date>`,
      },
    });
  });
});
