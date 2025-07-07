
import http from '@/api/http'

export const vectorPage = (vectorIndex,pageId,pageSize) =>http.get(`/vector/page?vectorIndex=${vectorIndex}&pageId=${pageId}&pageSize=${pageSize}`)

export const vectorDelete = (formData) =>http.post('/vector/deleteVector',formData,{headers: {'Content-Type': 'application/json'}});