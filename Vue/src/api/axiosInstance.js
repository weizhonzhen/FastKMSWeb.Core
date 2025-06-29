
import axios from 'axios';

const axiosInstance = axios.create({
  baseURL: 'http://localhost:5239/api/', 
  timeout: 60000,
});

axiosInstance.interceptors.request.use(
  config => {   
    const token = localStorage.getItem('token');
    if (token) config.headers.Authorization = `Bearer ${token}`;
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
    if (error.response) {
      switch (error.response.state) {
        case 401:
           window.location.href = "/login";
        default:
          return Promise.reject(error.response.data); 
      }
    } else 
     {    
           window.location.href = "/login";
          return Promise.reject(error.response.data); 
     }
  }
);


export default axiosInstance;