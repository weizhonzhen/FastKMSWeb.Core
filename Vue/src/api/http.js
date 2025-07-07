
import axios from 'axios';

const http = axios.create({
  baseURL: 'http://localhost:5239/api/', 
  timeout: 600000,
});

http.interceptors.request.use(
  config => {   
    const token = localStorage.getItem('token');
    if (token) config.headers.Authorization = `Bearer ${token}`;
    return config;
  },
  error => {    
    return Promise.reject(error);
  }
);

http.interceptors.response.use(
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


export default http;