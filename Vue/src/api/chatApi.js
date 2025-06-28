
import axiosInstance from '@/api/axiosInstance'

export const chatPage = (pageId,pageSize) =>axiosInstance.get(`/chat/page?pageId=${pageId}&pageSize=${pageSize}`)

export const chatDelete = (formData) =>axiosInstance.post('/chat/deleteChat',formData, {headers: {'Content-Type': 'multipart/form-data'}});

export const chatRecord = (chatIndex) =>axiosInstance.get(`/chat/chatRecord?chatIndex=${chatIndex}`)

export const nl2Sql = (formData) =>axiosInstance.post('/chat/nl2Sql',formData, {headers: {'Content-Type': 'application/json'}});

export const chat = (formData) =>axiosInstance.post('/chat/chat',formData, {headers: {'Content-Type': 'application/json'}});

