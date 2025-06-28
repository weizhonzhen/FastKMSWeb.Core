
import axiosInstance from '@/api/axiosInstance'

export const kmsPage = (pageId,pageSize) =>axiosInstance.get(`/kms/page?pageId=${pageId}&pageSize=${pageSize}`)

export const kmsList = () =>axiosInstance.get('/kms/list');

export const kmsDelete = (formData) =>axiosInstance.post('/kms/deleteVector',formData, {headers: {'Content-Type': 'application/json'}});

export const kmsUploadFile = (formData) =>axiosInstance.post('/kms/uploadFile',formData, {headers: {'Content-Type': 'multipart/form-data'}});

export const kmsUpdate = (formData) =>axiosInstance.post('/kms/update',formData, {headers: {'Content-Type': 'application/json'}});