import type { Config } from "tailwindcss";

export default {
  content: ["./index.html", "./src/**/*.{ts,tsx}"],
  theme: {
    extend: {
      colors: {
        brand: {
          50: "#eef8ff",
          600: "#0877b9",
          700: "#075f94"
        }
      }
    }
  },
  plugins: []
} satisfies Config;
