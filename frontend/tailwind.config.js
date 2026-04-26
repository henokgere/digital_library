/** @type {import('tailwindcss').Config} */
export default {
  darkMode: 'class', // enable dark mode via 'class'
  content: [
    "./index.html",
    "./src/**/*.{js,jsx,ts,tsx,css}", // scan all JS/TSX and CSS files
  ],
  theme: {
    extend: {
      colors: {
        primary: {
          DEFAULT: "#000000",
          dark: "#333333",
        },
        'hildana-pink': "#808080", // gray for highlights
        light: "#ffffff",          // white background
      },
      animation: {
        float: "float 6s ease-in-out infinite",
        move: 'move 8s linear infinite',
        'spin-slow': 'spin 8s linear infinite',
      },
      keyframes: {
        float: {
          "0%, 100%": { transform: "translateY(0px)" },
          "50%": { transform: "translateY(-15px)" },
        },
        move: {
          '0%': { transform: 'translateX(0px)' },
          '50%': { transform: 'translateX(20px)' },
          '100%': { transform: 'translateX(0px)' },
        },
      },
    },
  },
  plugins: [],
};
