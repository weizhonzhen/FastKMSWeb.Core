
import axios from 'axios';

const axiosInstance = axios.create({
  baseURL: 'http://localhost:5239/api/', 
  timeout: 600000, 
});

axiosInstance.interceptors.request.use(
  config => {
   

    return config;
  },
  error => {    
    console.log(error);    
    return Promise.reject(error);
  }
);

axiosInstance.interceptors.response.use(
  response => {
    

    return response;
  },
  error => {    
    console.log(error);    
    return Promise.reject(error);
  }
);

export default axiosInstance;