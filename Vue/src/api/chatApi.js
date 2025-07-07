
import http from '@/api/http'

export const chatPage = (pageId,pageSize) =>http.get(`/chat/page?pageId=${pageId}&pageSize=${pageSize}`)

export const chatDelete = (formData) =>http.post('/chat/deleteChat',formData, {headers: {'Content-Type': 'multipart/form-data'}});

export const chatRecord = (chatIndex) =>http.get(`/chat/chatRecord?chatIndex=${chatIndex}`)

export const nl2Sql = (formData) =>http.post('/chat/nl2Sql',formData, {headers: {'Content-Type': 'application/json'}});

export const chat = (formData) =>http.post('/chat/chat',formData, {headers: {'Content-Type': 'application/json'}});

export const mcp = (formData) =>http.post('/chat/mcp',formData, {headers: {'Content-Type': 'application/json'}});

