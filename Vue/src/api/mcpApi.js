
import http from '@/api/http'

export const mcpPage = (pageId,pageSize) =>http.get(`/mcp/page?pageId=${pageId}&pageSize=${pageSize}`)

export const mcpDelete = (formData) =>http.post('/mcp/delete',formData, {headers: {'Content-Type': 'application/json'}});

export const mcpUpdate = (formData) =>http.post('/mcp/update',formData, {headers: {'Content-Type': 'application/json'}});

export const mcpAdd = (formData) =>http.post('/mcp/add',formData, {headers: {'Content-Type': 'application/json'}});