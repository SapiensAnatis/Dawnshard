const tailwindcss = require('@tailwindcss/postcss');
const autoprefixer = require('autoprefixer');

const config = {
  plugins: [
    //Some plugins, like tailwindcss/nesting, need to run before Tailwind,
    tailwindcss(),
    //But others, like autoprefixer, need to run after,
    autoprefixer
  ]
};

module.exports = config;
