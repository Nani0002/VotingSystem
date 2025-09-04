using AutoMapper;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using VotingSystem.DataAccess.Models;
using VotingSystem.DataAccess.Services;
using VotingSystem.Shared.Models;
using VotingSystem.Shared.SignalR.Models;
using VotingSystem.SignalR.Services;

namespace VotingSystem.WebAPI.Controllers
{
    [Route("/topics")]
    [ApiController]
    public class TopicsController : ControllerBase
    {
        private readonly IMapper _mapper;
        private readonly ITopicsService _topicsService;
        private readonly IUsersService _usersService;
        private readonly ITopicsNotificationService _topicsNotificationService;

        public TopicsController(IMapper mapper, ITopicsService topicsService, IUsersService usersService, ITopicsNotificationService topicsNotificationService)
        {
            _mapper = mapper;
            _topicsService = topicsService;
            _usersService = usersService;
            _topicsNotificationService = topicsNotificationService;
        }

        [HttpPost]
        [ProducesResponseType(statusCode: StatusCodes.Status201Created, type: typeof(TopicResponseDto))]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status409Conflict)]
        public async Task<IActionResult> CreateTopic([FromBody] TopicRequestDto topicRequestDto)
        {
            var userId = _usersService.GetCurrentUserId()!;
            var topic = _mapper.Map<Topic>(topicRequestDto);
            await _topicsService.AddAsync(topic, userId);

            var topicResponseDto = _mapper.Map<TopicResponseDto>(topic);
            return CreatedAtAction(nameof(CreateTopic), new { id = topicResponseDto.Id }, topicResponseDto);
        }

        [HttpGet]
        [AllowAnonymous]
        [ProducesResponseType(statusCode: StatusCodes.Status200OK, type: typeof(List<TopicResponseDto>))]
        public async Task<IActionResult> GetUserTopics()
        {
            var userId = _usersService.GetCurrentUserId()!;
            var topics = await _topicsService.GetUsersAsync(userId);
            var topicResponseDtos = _mapper.Map<List<TopicResponseDto>>(topics);

            return Ok(topicResponseDtos);
        }

        [HttpGet]
        [Route("all")]
        [AllowAnonymous]
        [ProducesResponseType(statusCode: StatusCodes.Status200OK, type: typeof(List<TopicResponseDto>))]
        public async Task<IActionResult> GetAllTopics([FromQuery] string? name, [FromQuery] DateTime? startDate, [FromQuery] DateTime? endDate, [FromQuery] bool onlyOpen = false, [FromQuery] bool onlyClosed = false)
        {
            var topics = await _topicsService.GetAllAsync(name, startDate, endDate, onlyOpen, onlyClosed);
            var topicResponseDtos = _mapper.Map<List<TopicResponseDto>>(topics);

            return Ok(topicResponseDtos);
        }

        [HttpGet]
        [AllowAnonymous]
        [Route("{id}")]
        [ProducesResponseType(statusCode: StatusCodes.Status200OK, type: typeof(TopicResponseDto))]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<IActionResult> GetTopicById([FromRoute] long id)
        {
            var topic = await _topicsService.GetByIdAsync((int)id);
            var topicResponseDto = _mapper.Map<TopicResponseDto>(topic);

            return Ok(topicResponseDto);
        }

        [HttpPut]
        [Route("{id}")]
        [ProducesResponseType(statusCode: StatusCodes.Status200OK, type: typeof(TopicResponseDto))]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status403Forbidden)]
        [ProducesResponseType(StatusCodes.Status409Conflict)]
        public async Task<IActionResult> UpdateTopic([FromRoute] long id, [FromBody] TopicRequestDto topicRequestDto)
        {
            var userId = _usersService.GetCurrentUserId();
            if (userId == null)
            {
                return Unauthorized();
            }
            var existingTopic = await _topicsService.GetByIdAsync((int)id);
            if (existingTopic == null)
                return NotFound();

            if (existingTopic.OwnerId != userId)
                return Forbid();

            var topic = _mapper.Map<Topic>(topicRequestDto);

            if (existingTopic.StartDate <= DateTime.UtcNow &&
                (topicRequestDto.StartDate != existingTopic.StartDate || topicRequestDto.Name != existingTopic.Name ||
                 topicRequestDto.Description != existingTopic.Description || topicRequestDto.MinimalVotes != existingTopic.MinimalVotes ||
                 topicRequestDto.MaximalVotes != existingTopic.MaximalVotes || topicRequestDto.Live != existingTopic.Live))
            {
                return BadRequest("Topic has already started. Only the close date can be changed.");
            }

            if (existingTopic.Votes.Any())
            {
                if (topicRequestDto.CloseDate <= existingTopic.CloseDate)
                    return BadRequest("Cannot set close date earlier than current. Votes have already been submitted.");

                if (!topicRequestDto.Choices.SequenceEqual(existingTopic.Choices.Select(c => c.Value)))
                    return BadRequest("Cannot modify choices after votes have been cast.");
            }

            topic.Id = (int)id;
            topic.OwnerId = userId;

            await _topicsService.UpdateAsync(topic);

            var topicResponseDto = _mapper.Map<TopicResponseDto>(topic);

            return Ok(topicResponseDto);
        }


        [HttpDelete]
        [Route("{id}")]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(StatusCodes.Status403Forbidden)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<IActionResult> DeleteTopic([FromRoute] long id)
        {
            var topic = await _topicsService.GetByIdAsync((int)id);
            if (topic == null) return NotFound();

            var userId = _usersService.GetCurrentUserId();
            if (topic.OwnerId != userId) return Forbid();

            await _topicsService.DeleteAsync((int)id);
            return NoContent();
        }

        [HttpPost]
        [Route("vote")]
        [ProducesResponseType(StatusCodes.Status201Created)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status409Conflict)]
        public async Task<IActionResult> VoteOnTopic([FromBody] VoteRequestDto voteRequestDto)
        {
            var userId = _usersService.GetCurrentUserId();
            if (userId == null)
            {
                return Unauthorized();
            }

            var topic = await _topicsService.GetByIdAsync(voteRequestDto.TopicId);

            if (topic == null) return NotFound();

            var selectedChoices = topic.Choices.Where(c => voteRequestDto.SelectedChoices.Contains(c.Value)).ToList();

            var vote = new Vote
            {
                TopicId = topic.Id,
                Topic = topic,
                SelectedChoices = selectedChoices,
                CreatedAt = DateTime.Now
            };

            var record = new VoteRecord
            {
                UserId = userId,
                TopicId = topic.Id,
                Topic = topic
            };

            await _topicsService.VoteOnTopicAsync(vote, record);

            var updatedTopic = await _topicsService.GetByIdAsync(vote.TopicId);
            var notificationDto = _mapper.Map<TopicNotificationDto>(updatedTopic);

            await _topicsNotificationService.SendVoteUpdateToGroupAsync(vote.TopicId, notificationDto);

            return CreatedAtAction(nameof(VoteOnTopic), null);
        }
    }
}
