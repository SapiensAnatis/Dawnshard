import dotenv from 'dotenv';

const globalSetup = () => {
  dotenv.config({
    path: ['tests/.env.local']
  });
};

export default globalSetup;
