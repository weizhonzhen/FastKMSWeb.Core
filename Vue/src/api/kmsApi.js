
import http from '@/api/http'

export const kmsPage = (pageId,pageSize) =>http.get(`/kms/page?pageId=${pageId}&pageSize=${pageSize}`)

export const kmsList = () =>http.get('/kms/list');

export const kmsDelete = (formData) =>http.post('/kms/deleteVector',formData, {headers: {'Content-Type': 'application/json'}});

export const kmsUploadFile = (formData) =>http.post('/kms/uploadFile',formData, {headers: {'Content-Type': 'multipart/form-data'}});

export const kmsUpdate = (formData) =>http.post('/kms/update',formData, {headers: {'Content-Type': 'application/json'}});