
import axiosInstance from '@/api/axiosInstance'

export const vectorPage = (vectorIndex,pageId,pageSize) =>axiosInstance.get(`/vector/page?vectorIndex=${vectorIndex}&pageId=${pageId}&pageSize=${pageSize}`)

export const vectorDelete = (formData) =>axiosInstance.post('/vector/deleteVector',formData,{headers: {'Content-Type': 'application/json'}});